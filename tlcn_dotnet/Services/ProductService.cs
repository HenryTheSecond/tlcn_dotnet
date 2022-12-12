using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.ServicesImpl
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductImageService _productImageService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductService(IMapper mapper, IProductImageService productImageService, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _productImageService = productImageService;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;   
        }
        public async Task<DataResponse> AddProduct(AddProductDto addProductDto, IFormFileCollection images)
        {
            Product product = _mapper.Map<Product>(addProductDto);
            product.Category = await _categoryRepository.GetById(addProductDto.CategoryId.Value);
            Product productDb = await _productRepository.Add(product);
            var productImages = await _productImageService.AddProductImages(images, productDb);
            var productWithImageDto = _mapper.Map<ProductWithImageDto>(productDb);
            productWithImageDto.ProductImages = productImages;
            return new DataResponse(productWithImageDto);
        }

        public async Task<DataResponse> DeleteProduct(long? id)
        {
            await _productImageService.DeleteAllImageOfProduct(id);
            await _productRepository.Remove(id.Value);
            return new DataResponse(true);
        }

        public async Task<DataResponse> EditProduct(long? id, EditProductDto editProductDto, IFormFileCollection files)
        {
            Product product = await _productRepository.GetById(id.Value);
            if(product == null) throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            Category category = await _categoryRepository.GetById(editProductDto.CategoryId.Value);
            if(category == null) throw new GeneralException("CATEGORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            product.Name = editProductDto.Name;
            product.Price = editProductDto.Price != null ? editProductDto.Price : product.Price;
            product.Unit = editProductDto.Unit;
            product.MinPurchase = editProductDto.MinPurchase != null ? editProductDto.MinPurchase : product.MinPurchase;
            product.Status = editProductDto.Status;
            product.Description = editProductDto.Description != null ? editProductDto.Description : product.Description;
            product.Quantity = editProductDto.Quantity != null ? editProductDto.Quantity : product.Quantity;
            product.Category = category;

            IEnumerable<SimpleProductImageDto> productImages = await _productImageService.EditProductImage(product, editProductDto.EditImageStatus, files);
            ProductWithImageDto productWithImageDto = _mapper.Map<ProductWithImageDto>(product);
            productWithImageDto.ProductImages = productImages;
            await _productRepository.Update(product);
            return new DataResponse(productWithImageDto);
        }

        public async Task<DataResponse> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice,
            long? categoryId, ProductOrderBy? productOrderBy, SortOrder? sortOrder, int page)
        {
            var result = await _productRepository.FilterProduct(keyword, minPrice, maxPrice, categoryId, 
                productOrderBy, sortOrder, page);
            var products = _mapper.Map<List<SingleImageProductDto>>(result.Products);
            var maxPage = Util.CalculateMaxPage(result.Total, 2);

            return new DataResponse(new { 
                products = products,
                maxPage = maxPage,
                currentPage = page
            });
        }

        public async Task<DataResponse> GetAllProductIdAndNameAndUnit()
        {
            var products = await _productRepository.GetAll();
            return new DataResponse
                (_mapper.Map<IEnumerable<ProductWithIdNameUnitDto>>(products));
        }

        public async Task<DataResponse> GetBestProduct()
        {
            var product = await _productRepository.GetBestProduct();
            return new DataResponse(_mapper.Map<SingleImageProductDto>(product));
        }

        public async Task<DataResponse> GetProductById(long? id)
        {
            Product product = await _productRepository.GetProductWithImageById(id);
            if (product == null)
                throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            ProductWithImageDto productWithImageDto = _mapper.Map<ProductWithImageDto>(product);
            return new DataResponse(productWithImageDto);
        }

        public async Task<DataResponse> GetTop8Product()
        {
            var products = await _productRepository.GetTop8Product();
            return new DataResponse(_mapper.Map<IList<SingleImageProductDto>>(products));
        }
    }
}
