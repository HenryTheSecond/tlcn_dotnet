using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.BillDetailDto;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Dto.CartNotificationDto;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Dto.ProductPromotionDto;
using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.Dto.SupplierDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Mapper
{
    public class ManageMapper : Profile
    {
        public ManageMapper()
        {
            CreateMap<Category, SimpleCategoryDto>();
            CreateMap<SimpleCategoryDto, Category>();

            CreateMap<RegisterAccountDto, Account>();
            CreateMap<Account, AccountResponse>();
            CreateMap<Account, AccountWithProviderResponse>();
            CreateMap<Account, AccountReviewDto>();
            CreateMap<Account, EmployeeResponse>()
                .ForMember(dest => dest.Salary, map => map.MapFrom(src => src.Employee != null ? src.Employee.Salary : 0));

            CreateMap<Supplier, SimpleSupplierDto>();
            CreateMap<AddSupplierDto, Supplier>();
            CreateMap<Supplier, SupplierIdAndName>();

            CreateMap<AddProductDto, Product>();
            CreateMap<Product, SimpleProductDto>()
                .AfterMap((src, dest) =>
                {
                    if (src.Reviews != null)
                    {
                        dest.Rating = src.Reviews.Average(review => review.Rating);
                    }
                });
            CreateMap<Product, ProductWithImageDto>()
                .AfterMap((src, dest) =>
                {
                    if (src.Reviews != null)
                    {
                        dest.Rating = src.Reviews.Average(review => review.Rating);
                    }
                });
            //CreateMap<Product, SingleImageProductDto>();
            CreateMap<Product, SingleImageProductDto>()
                .AfterMap((src, dest) =>
                {
                    ProductImage image = src.ProductImages.FirstOrDefault();
                    if (image != null)
                    {
                        dest.Image = new SimpleProductImageDto
                        {
                            Id = image.Id,
                            FileName = image.FileName,
                            ProductId = src.Id,
                            Url = image.Url
                        };
                    }

                    if (src.Reviews != null)
                    {
                        dest.Rating = src.Reviews.Average(review => review.Rating);
                    }
                });
            CreateMap<ProductImage, SimpleProductImageDto>();
            CreateMap<Product, ProductIdAndNameDto>();
            CreateMap<Product, ProductWithIdNameUnitDto>();

            CreateMap<Inventory, SimpleInventoryDto>();

            CreateMap<CartDetail, CartDetailResponse>();

            CreateMap<Cart, CartResponse>();

            CreateMap<Bill, SimpleBillDto>();
            CreateMap<Bill, BillWithBillDetailDto>();

            CreateMap<BillDetail, BillDetailWithProductDto>();

            CreateMap<Review, ReviewResponse>();

            CreateMap<CartNotification, CartNotificationResponse>();

            CreateMap<ProductPromotionAddRequest, ProductPromotion>();
            CreateMap<ProductPromotion, ProductPromotionResponse>()
                .ForMember(p => p.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<ProductPromotion, SimpleProductPromotionDto>();
            
        }
    }
}
