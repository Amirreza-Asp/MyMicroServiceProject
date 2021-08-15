using Discount.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _dpService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _dpService = discountProtoService;
        }

        public async Task<CouponModel> GetDiscountAsync(string productName)
        {
            var request = new GetDiscountRequest { ProductName = productName };
            var obj =  await  _dpService.GetDiscountAsync(request);
            return obj;
        }

    }
}
