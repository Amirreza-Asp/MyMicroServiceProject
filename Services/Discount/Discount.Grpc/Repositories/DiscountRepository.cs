using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> AddCouponAsync(Coupon coupon)
        {
            using var connection =
                new NpgsqlConnection(_configuration.GetValue<String>("DatabaseSettings:ConnectionString"));

            var effected =
               await connection.ExecuteAsync(
                   "INSERT INTO Coupon(ProductName,Description,Amount) VALUES (@ProductName,@Description,@Amount)",
                        new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return effected != 0;
        }

        public async Task<Coupon> GetCouponAsync(String productName)
        {
            using var connection =
                new NpgsqlConnection(_configuration.GetValue<String>("DatabaseSettings:ConnectionString"));

            var coupon =
                await connection.QueryFirstOrDefaultAsync<Coupon>(
                    "SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            if (coupon == null)
                return new Coupon
                { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            return coupon;
        }

        public async Task<bool> RemoveCouponAsync(String productName)
        {
            using var connection =
                new NpgsqlConnection(_configuration.GetValue<String>("DatabaseSettings:ConnectionString"));

            var effected =
                await connection.ExecuteAsync(
                    "DELETE FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            return effected != 0;
        }

        public async Task<bool> UpdateCouponAsync(Coupon coupon)
        {
            using var connection =
                new NpgsqlConnection(_configuration.GetValue<String>("DatabaseSettings:ConnectionString"));

            var effected =
                await connection.ExecuteAsync(
                    "UPDATE Coupon SET ProductName = @ProductName , Description = @Description , Amount = @Amount WHERE Id = @Id",
                         new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            return effected != 0;
        }
    }
}
