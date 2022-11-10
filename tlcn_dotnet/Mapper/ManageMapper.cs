using AutoMapper;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
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

            CreateMap<Supplier, SimpleSupplierDto>();
            CreateMap<AddSupplierDto, Supplier>();

            CreateMap<AddProductDto, Product>();
            CreateMap<Product, SimpleProductDto>();
            CreateMap<Product, SingleImageProductDto>();
            /*CreateMap<Product, SingleImageProductDto>()
                .ForMember(dest => dest.Image, b => b.MapFrom(src => src.ProductImages.FirstOrDefault()));*/

            CreateMap<ProductImage, SimpleProductImageDto>();
        }
    }
}
