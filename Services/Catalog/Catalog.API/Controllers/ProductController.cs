using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _prodRepo;

        public ProductController(IProductRepository productRepository)
        {
            _prodRepo = productRepository;
        }

        /// <summary>
        /// Get the list of Products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _prodRepo.GetProducts();
            return Ok(products);
        }

        /// <summary>
        /// Get the product by indiviual Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(String id)
        {
            var product = await _prodRepo.GetProductById(id);

            if (product == null)
            {
                ModelState.AddModelError("", "Product not found");
            }

            return Ok(product);
        }

        /// <summary>
        /// Get the list of products by category name
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [Route("[action]/{category}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsByCategory(String category)
        {
            var products = await _prodRepo.GetProductsByCategory(category);
            return Ok(products);
        }

        /// <summary>
        /// Add a new Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _prodRepo.AddProduct(product);
            return CreatedAtAction(nameof(GetProductById), product.Id);
        }

        /// <summary>
        /// Update a product when Product exists
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            if(await _prodRepo.UpdateProduct(product))
            {
                return StatusCode(204, "Update Product Suceesfully");
            }

            ModelState.AddModelError("", $"Somthing went wrong when Updating the record ${product.Name}");
            return StatusCode(500, ModelState);
        }

        /// <summary>
        /// Delete Product by individual Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProduct(String id)
        {
            if(await _prodRepo.RemoveProduct(id))
            {
                return StatusCode(204, "Remove product successfully");
            }

            ModelState.AddModelError("", "Product Id is wrong");
            return StatusCode(400, ModelState);
        }
    }
}
