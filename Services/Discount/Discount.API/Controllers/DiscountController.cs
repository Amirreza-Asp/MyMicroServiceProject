using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _disRepo;

        public DiscountController(IDiscountRepository discountRepository)
        {
            _disRepo = discountRepository;
        }

        [HttpGet("{productName}" , Name = "GetDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDiscount(String productName)
        {
            var coupon =await _disRepo.GetCouponAsync(productName);

            if (coupon == null)
            {
                ModelState.AddModelError("", "Disocunt not found");
                return NotFound(ModelState);
            }

            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDiscount([FromBody] Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _disRepo.AddCouponAsync(coupon))
            {
                return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
            }

            ModelState.AddModelError("", $"Somthing went wrong when Saving the record {coupon.ProductName}");
            return StatusCode(500, ModelState);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDiscount([FromBody] Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _disRepo.UpdateCouponAsync(coupon))
            {
                return StatusCode(204);
            }

            ModelState.AddModelError("", $"Somthing went wrong when Updating the record {coupon.ProductName}");
            return StatusCode(500, ModelState);
        }

        [HttpDelete("{productName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveDiscount(String productName)
        {
            if(await _disRepo.RemoveCouponAsync(productName))
            {
                return StatusCode(204);
            }

            ModelState.AddModelError("", $"Somthing went wrong when Removing the Discount");
            return StatusCode(500, ModelState);
        }
    }
}
