using Basket.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        public Task<ShoppingCart> GetBasket(String username);

        public Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart);

        public Task RemoveBasket(String username);
    }
}
