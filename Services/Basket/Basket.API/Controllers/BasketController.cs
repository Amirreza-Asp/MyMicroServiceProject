using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
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

        public BasketController(IBasketRepository basketRepository , DiscountGrpcService discountGrpcService)
        {
            _basketRepo = basketRepository;
            _disGrpcServ = discountGrpcService;
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

        [HttpPut]
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
            return StatusCode(204, "Basket removed successfully");
        }
    }
}
