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

            //CreateMap<IList<ProductImage>, IList<SimpleProductImageDto>>();
            CreateMap<ProductImage, SimpleProductImageDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(_ => _.Id))
                .ForMember(dest => dest.Url, src => src.MapFrom(_ => _.Url))
                .ForPath(dest => dest.ProductId, src => src.MapFrom(_ => _.Product.Id));
        }
    }
}
