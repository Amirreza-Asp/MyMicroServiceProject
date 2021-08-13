using Catalog.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProductsByName(String name);
        Task<IEnumerable<Product>> GetProductsByCategory(String category);
        Task<Product> GetProductById(String id);

        Task AddProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> RemoveProduct(String id);
    }
}
