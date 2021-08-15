using Discount.Grpc.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public interface IDiscountRepository
    {
        public Task<Coupon> GetCouponAsync(String productName);

        public Task<bool> AddCouponAsync(Coupon coupon);
        public Task<bool> UpdateCouponAsync(Coupon coupon);
        public Task<bool> RemoveCouponAsync(String productName);

    }
}
