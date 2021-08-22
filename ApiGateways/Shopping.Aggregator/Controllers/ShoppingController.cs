using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly ICatalogService _catalogService;

        public ShoppingController(IBasketService basketService, IOrderService orderService, ICatalogService catalogService)
        {
            _basketService = basketService;
            _orderService = orderService;
            _catalogService = catalogService;
        }

        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShopping(String username)
        {
            var basket = await _basketService.GetBasket(username);

            foreach(var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);

                // set additional product fileds
                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summary = product.Summary;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            var orders = await _orderService.GetOrdersByUsername(username);

            var shoppingModel = new ShoppingModel
            {
                UserName = username,
                Orders = orders,
                BasketWithProducts = basket
            };

            return Ok(shoppingModel);
        }
    }
}
