using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

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
            var productWithImageDto = _mapper.Map<ProductWithImageDto>(productDb);
            productWithImageDto.ProductImages = productImages;
            return new DataResponse(productWithImageDto);
        }

        public async Task<DataResponse> DeleteProduct(long? id)
        {
            try
            {
                _dbContext.Remove(_dbContext.Product.Single(product => product.Id == id));

                await _productImageService.DeleteAllImageOfProduct(id);

                await _dbContext.SaveChangesAsync();
                return new DataResponse(true);
            }
            catch (InvalidOperationException e)
            {
                throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            }
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
            product.Status = editProductDto.Status;
            product.Description = editProductDto.Description != null ? editProductDto.Description : product.Description;
            product.Category = category;

            IEnumerable<SimpleProductImageDto> productImages = await _productImageService.EditProductImage(product, editProductDto.EditImageStatus, files);
            ProductWithImageDto productWithImageDto = _mapper.Map<ProductWithImageDto>(product);
            productWithImageDto.ProductImages = productImages;
            await _dbContext.SaveChangesAsync();
            return new DataResponse(productWithImageDto);
        }

        public async Task<DataResponse> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice, long? categoryId, int page)
        {
            IQueryable<Product> queryProduct = _dbContext.Product
                .Include(product => product.Category)
                .Include(product => product.ProductImages);

            if (keyword != null)
                queryProduct = queryProduct.Where(product => product.Name.Contains(keyword) || product.Description.Contains(keyword));
            if (minPrice != null)
                queryProduct = queryProduct.Where(product => product.Price >= minPrice);
            if (maxPrice != null)
                queryProduct = queryProduct.Where(product => product.Price <= maxPrice);
            if (categoryId != null)
                queryProduct = queryProduct.Where(product => product.Category.Id == categoryId);


            var listProduct = _mapper.Map<List<SingleImageProductDto>>(await queryProduct.Skip((page - 1) * 2).Take(2).ToListAsync());
            var maxPage = Util.CalculateMaxPage((await queryProduct.LongCountAsync()), 2);

            return new DataResponse(new { 
                products = listProduct,
                maxPage = maxPage,
                currentPage = page
            });
        }

        public async Task<DataResponse> GetProductById(long? id)
        {
            Product product = await _dbContext.Product.Include(product => product.ProductImages)
                .Include(product => product.Category)
                .FirstOrDefaultAsync(product => product.Id == id);
            if (product == null)
                throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            ProductWithImageDto productWithImageDto = _mapper.Map<ProductWithImageDto>(product);
            return new DataResponse(productWithImageDto);
        }
    }
}
