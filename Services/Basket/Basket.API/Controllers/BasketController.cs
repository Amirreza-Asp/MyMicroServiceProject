using Basket.API.Entities;
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

        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepo = basketRepository;
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart basket)
        {
            var obj = await _basketRepo.UpdateBasket(basket);

            if (obj == null)
            {
                ModelState.AddModelError("", $"Somthing went wrong when Editing the record {basket.UserName}");
                return BadRequest(ModelState);
            }

            return StatusCode(204, obj);
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
