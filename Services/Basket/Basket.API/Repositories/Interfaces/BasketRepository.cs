using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories.Interfaces
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<ShoppingCart> GetBasket(string username)
        {
            var jsonBasket = await _redisCache.GetStringAsync(username);

            if (!String.IsNullOrWhiteSpace(jsonBasket))
            {
                return JsonConvert.DeserializeObject<ShoppingCart>(jsonBasket);
            }

            return null;
        }

        public async Task RemoveBasket(string username)
        {
            await _redisCache.RemoveAsync(username);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart)
        {
            String jsonBasket = JsonConvert.SerializeObject(shoppingCart);
            await _redisCache.SetStringAsync(shoppingCart.UserName, jsonBasket);

            return await GetBasket(shoppingCart.UserName);
        }
    }
}
