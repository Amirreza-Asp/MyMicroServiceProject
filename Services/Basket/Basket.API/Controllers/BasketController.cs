using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepo;
        private readonly DiscountGrpcService _disGrpcServ;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;  

        public BasketController(IBasketRepository basketRepository , DiscountGrpcService discountGrpcService , IMapper mapper , IPublishEndpoint publishEndpoint)
        {
            _basketRepo = basketRepository;
            _disGrpcServ = discountGrpcService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBasket(String username)
        {
            var obj = await _basketRepo.GetBasket(username);

            if (obj == null)
            {
                ModelState.AddModelError("", "not find basket with entered username");
                return NotFound(ModelState);
            }

            return Ok(obj);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart basket)
        {
            if (basket == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach(var item in basket.Items)
            {
                var coupon = await _disGrpcServ.GetDiscountAsync(item.ProductName);
                item.Price -= coupon.Amount;
            }

            var obj = await _basketRepo.UpdateBasket(basket);
            return Ok(obj);
        }

        [HttpDelete("username")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveBasket(String username)
        {
            await _basketRepo.RemoveBasket(username);
            return StatusCode(204);
        }
        
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get basket form database with total price
            var basket = await _basketRepo.GetBasket(basketCheckout.UserName);
            if(basket == null)
            {
                ModelState.AddModelError("",$"Not found any Basket with username: {basketCheckout.UserName}");
                return NotFound(ModelState);
            }

            // Publish checkout event to rabbitmq
            var basketEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            basketEvent.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(basketEvent);

            // Remove the basket
            await _basketRepo.RemoveBasket(basket.UserName);

            return Accepted();
        }
    }
}
