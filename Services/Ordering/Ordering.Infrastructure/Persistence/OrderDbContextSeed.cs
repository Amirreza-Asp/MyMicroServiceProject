using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence.Data
{
    public static class OrderDbContextSeed
    {
        public static  void SeedAsync(OrderDbContext orderContext)
        {
            if (!orderContext.Order.Any())
            {
                orderContext.Order.AddRange(GetPreconfiguredOrders());
                orderContext.SaveChanges();
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() {UserName = "designer", FirstName = "amir", LastName = "mohammadi", EmailAddress = "m.senator.6247@gmail.com", AddressLine = "blablabla", Country = "iran", TotalPrice = 350 }
            };
        }
    }
}
