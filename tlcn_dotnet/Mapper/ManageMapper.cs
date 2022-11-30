using AutoMapper;
using tlcn_dotnet.Dto;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.Dto.SupplierDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Mapper
{
    public class ManageMapper: Profile
    {
        public ManageMapper()
        {
            CreateMap<Category, SimpleCategoryDto>();
            CreateMap<SimpleCategoryDto, Category>();

            CreateMap<RegisterAccountDto, Account>();
            CreateMap<Account, AccountResponse>();
            CreateMap<Account, AccountReviewDto>();

            CreateMap<Supplier, SimpleSupplierDto>();
            CreateMap<AddSupplierDto, Supplier>();
            CreateMap<Supplier, SupplierIdAndName>();

            CreateMap<AddProductDto, Product>();
            CreateMap<Product, SimpleProductDto>();
            CreateMap<Product, ProductWithImageDto>();
            //CreateMap<Product, SingleImageProductDto>();
            CreateMap<Product, SingleImageProductDto>()
                .AfterMap((src, dest) =>
                {
                    ProductImage image = src.ProductImages.FirstOrDefault();
                    if (image == null)
                        return;
                    dest.Image = new SimpleProductImageDto
                    {
                        Id = image.Id,
                        FileName = image.FileName,
                        ProductId = src.Id,
                        Url = image.Url
                    };
                });
            CreateMap<ProductImage, SimpleProductImageDto>();
            CreateMap<Product, ProductIdAndNameDto>();

            CreateMap<Inventory, SimpleInventoryDto>();

            CreateMap<CartDetail, CartDetailResponse>();

            CreateMap<Cart, CartResponse>();

            CreateMap<Bill, SimpleBillDto>();

            CreateMap<Review, ReviewResponse>();
        }
    }
}
