using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _disRepo;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _disRepo = discountRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _disRepo.GetCouponAsync(request.ProductName);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName{request.ProductName} is not found"));
            }

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            bool success = await _disRepo.AddCouponAsync(coupon);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Somthing went wrong when Saving the record {coupon.ProductName}"));
            }

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            bool success = await _disRepo.UpdateCouponAsync(coupon);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Somthing went wrong when Updating the record {coupon.ProductName}"));
            }

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<RemoveDiscountResponse> RemoveDiscount(RemoveDiscountRequest request, ServerCallContext context)
        {
            var success = await _disRepo.RemoveCouponAsync(request.ProductName);
            var response = new RemoveDiscountResponse()
            {
                Success = success
            };
            return response;
        }
    }
}
