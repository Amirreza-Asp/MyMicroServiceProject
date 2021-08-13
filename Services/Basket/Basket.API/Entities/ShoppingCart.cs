using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public String UserName { get; set; }

        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public ShoppingCart() { }

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }

        public Decimal TotalPrice
        {
            get
            {
                decimal totalPrice = 0;
                Items.ForEach(p => totalPrice += p.Price * p.Quantity);
                return totalPrice;
            }
        }

    }
}
