using AutoMapper;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Dto.InventoryDto;
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

            CreateMap<Inventory, SimpleInventoryDto>();
        }
    }
}
