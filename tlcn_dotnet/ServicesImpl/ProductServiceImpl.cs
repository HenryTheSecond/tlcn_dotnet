using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class ProductServiceImpl : ProductService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ProductImageService _productImageService;

        public ProductServiceImpl(MyDbContext dbContext, IMapper mapper, ProductImageService productImageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productImageService = productImageService;
        }
        public async Task<DataResponse> AddProduct(AddProductDto addProductDto, IFormFileCollection images)
        {
            Product product = _mapper.Map<Product>(addProductDto);
            product.Category = await _dbContext.Category.FindAsync(addProductDto.CategoryId);
            Product productDb = (await _dbContext.Product.AddAsync(product)).Entity;
            await _dbContext.SaveChangesAsync();
            var productImages = await _productImageService.AddProductImages(images, productDb);
            var simpleProductDto = _mapper.Map<SimpleProductDto>(productDb);
            return new DataResponse(new {
                product = simpleProductDto,
                images = productImages
            });
        }

        public async Task<DataResponse> EditProduct(long id, EditProductDto editProductDto, IFormFileCollection files)
        {
            Product product = await _dbContext.Product.FindAsync(id);
            if(product == null) throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            Category category = await _dbContext.Category.FindAsync(editProductDto.CategoryId);
            if(category == null) throw new GeneralException("CATEGORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            product.Name = editProductDto.Name;
            product.Price = editProductDto.Price != null ? editProductDto.Price : product.Price;
            product.Quantity = editProductDto.Quantity != null ? editProductDto.Quantity : product.Quantity;
            product.Unit = editProductDto.Unit;
            product.MinPurchase = editProductDto.MinPurchase != null ? editProductDto.MinPurchase : product.MinPurchase;
            product.Description = editProductDto.Description != null ? editProductDto.Description : product.Description;
            product.Category = category;

            //TODO edit image
            IEnumerable<SimpleProductImageDto> productImages = await _productImageService.EditProductImage(product, editProductDto.EditImageStatus, files);

            await _dbContext.SaveChangesAsync();
            return new DataResponse(new { 
                product = _mapper.Map<SimpleProductDto>(product),
                images = productImages
            });
        }

        public async Task<DataResponse> GetProductById(long? id)
        {
            Product product = await _dbContext.Product.FindAsync(id);
            if (product == null)
                throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            SimpleProductDto simpleProductDto = _mapper.Map<SimpleProductDto>(product);
            return new DataResponse(new
            {
                product = simpleProductDto,
                images = await _productImageService.GetImageByProduct(id)
            }); ;
        }
    }
}
