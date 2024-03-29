﻿using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Dto.ProductPromotionDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        public ProductRepository(MyDbContext dbContext, DapperContext dapperContext, IMapper mapper): base(dbContext)
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
        }

        public async Task<bool> CheckAccountBuyItem(long accountId, long productId)
        {
            int count = _dbContext.CartDetail.Include(cd => cd.Product)
                .Include(cd => cd.Cart)
                .Include(cd => cd.Account)
                .Where(cd => cd.Account.Id == accountId && cd.Product.Id == productId && cd.Cart.Status == CartStatus.DELIVERIED).Count();
            return count > 0;
        }

        public async Task<bool> DeleteProduct(long id)
        {
            var product = await _dbContext.Product.FindAsync(id);
            if (product == null)
                return false;
            product.IsDeleted = !product.IsDeleted;
            if(product.IsDeleted == true)
            {
                var cartDetails = await _dbContext.CartDetail.Where(cd => cd.CartId == null).ToListAsync();
                _dbContext.CartDetail.RemoveRange(cartDetails);
            }
            int affectedRows = await _dbContext.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<dynamic> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice,
            long? categoryId, ProductOrderBy? productOrderBy, SortOrder? sortOrder, int page, int pageSize, bool? isDeleted)
        {
            IQueryable<Product> queryProduct = _dbContext.Product
                .Include(product => product.Category)
                .Include(product => product.ProductImages)
                .Include(product => product.Reviews)
                .AsSplitQuery();

            if (keyword != null)
                queryProduct = queryProduct.Where(product => product.Name.Contains(keyword) || product.Description.Contains(keyword));
            if (minPrice != null)
                queryProduct = queryProduct.Where(product => product.Price >= minPrice);
            if (maxPrice != null)
                queryProduct = queryProduct.Where(product => product.Price <= maxPrice);
            if (categoryId != null)
                queryProduct = queryProduct.Where(product => product.Category.Id == categoryId);
            if(isDeleted != null)
            {
                queryProduct = queryProduct.Where(product => product.IsDeleted == isDeleted);
            }

            if (productOrderBy == null || productOrderBy == ProductOrderBy.ID)
                queryProduct = sortOrder == SortOrder.ASC ? 
                    queryProduct.OrderBy(product => product.Id) : 
                    queryProduct.OrderByDescending(product => product.Id);
            else if(productOrderBy == ProductOrderBy.PRICE)
                queryProduct = sortOrder == SortOrder.ASC ?
                    queryProduct.OrderBy(product => product.Price) :
                    queryProduct.OrderByDescending(product => product.Price);

            var queryProductAndPromotion = from product in queryProduct
                                           from promotion in _dbContext.ProductPromotion.Where(p => p.ProductId == product.Id && p.ExpireDate > DateTime.Now && p.IsEnable == true).Take(1).DefaultIfEmpty()
                                           select new {Product = _mapper.Map<SingleImageProductDto>(product), Promotion = promotion };
            IEnumerable<SingleImageProductDto> products = (await queryProductAndPromotion.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync()).Select(row =>
            {
                row.Product.Promotion = _mapper.Map<SimpleProductPromotionDto>(row.Promotion);
                return row.Product;
            });
            long total = await queryProduct.LongCountAsync();
            return new
            {
                Products = products,
                Total = total
            };
        }

        public async Task<IList<Product>> GetAllProudctWithImage()
        {
            return await _dbContext.Product.Include(product => product.ProductImages).ToListAsync();
        }

        public async Task<SingleImageProductDto> GetBestProduct()
        {
            string query = $@"SELECT TOP 1 Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity, Sales, Product.Description,
										AVG(Review.Rating) as Rating,
			                            Image.Id, Image.Url, Image.FileName,
										Category.Id, Category.Name,
										Promotion.Id, Promotion.Type, Promotion.CreatedDate, Promotion.ExpireDate, Promotion.IsEnable, Promotion.Value
                            FROM Product
							LEFT OUTER JOIN Category ON Product.CategoryId = Category.Id
							LEFT OUTER JOIN Review ON Review.ProductId = Product.Id
                            OUTER APPLY (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url FROM ProductImage where ProductImage.ProductId = Product.Id) as Image
							OUTER APPLY (SELECT TOP 1 pp.Id, pp.Type, pp.CreatedDate, pp.ExpireDate, pp.IsEnable, pp.Value FROM ProductPromotion pp WHERE pp.ProductId = Product.Id AND pp.ExpireDate > @Now AND pp.IsEnable = 1 ) AS Promotion
                            WHERE Product.IsDeleted = 0
                            GROUP BY Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity, Sales, Product.Description,
			                            Image.Id, Image.Url, Image.FileName, Category.Id, Category.Name,
										Promotion.Id, Promotion.Type, Promotion.CreatedDate, Promotion.ExpireDate, Promotion.IsEnable, Promotion.Value
                            ORDER BY CASE Product.MinPurchase
                                            WHEN 0 THEN Sales/1
                                            ELSE Sales/Product.MinPurchase
                                     END
                            DESC";
            using (var connection = _dapperContext.CreateConnection())
            {
                var products = await connection.QueryAsync<SingleImageProductDto, SimpleProductImageDto, SimpleCategoryDto, SimpleProductPromotionDto, SingleImageProductDto >(query, (product, image, category, promotion) =>
                {
                    product.Category = category;
                    product.Image = image;
                    product.Promotion = promotion;
                    return product;
                }, param: new { Now = DateTime.Now});
                return products.SingleOrDefault();
            }
        }

        public async Task<Product> GetProductWithImageById(long? id)
        {
            return await _dbContext.Product.Include(product => product.ProductImages)
                .Include(product => product.Category)
                .Include(product => product.Reviews)
                .FirstOrDefaultAsync(product => product.Id == id);
        }

        public async Task<IList<SingleImageProductDto>> GetTop8Product()
        {
            string query = $@"SELECT TOP 8 Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity, Sales, Product.Description,
										AVG(Review.Rating) as Rating,
			                            Image.Id, Image.Url, Image.FileName,
										Category.Id, Category.Name,
										Promotion.Id, Promotion.Type, Promotion.CreatedDate, Promotion.ExpireDate, Promotion.IsEnable, Promotion.Value
                            FROM Product
							LEFT OUTER JOIN Category ON Product.CategoryId = Category.Id
							LEFT OUTER JOIN Review ON Review.ProductId = Product.Id
                            OUTER APPLY (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url FROM ProductImage where ProductImage.ProductId = Product.Id) as Image
							OUTER APPLY (SELECT TOP 1 pp.Id, pp.Type, pp.CreatedDate, pp.ExpireDate, pp.IsEnable, pp.Value FROM ProductPromotion pp WHERE pp.ProductId = Product.Id AND pp.ExpireDate > @Now AND pp.IsEnable = 1 ) AS Promotion
                            WHERE Product.IsDeleted = 0
                            GROUP BY Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity, Sales, Product.Description,
			                            Image.Id, Image.Url, Image.FileName, Category.Id, Category.Name,
										Promotion.Id, Promotion.Type, Promotion.CreatedDate, Promotion.ExpireDate, Promotion.IsEnable, Promotion.Value
                            ORDER BY CASE Product.MinPurchase
                                            WHEN 0 THEN Sales/1
                                            ELSE Sales/Product.MinPurchase
                                     END
                            DESC";
            using (var connection = _dapperContext.CreateConnection())
            {
                var products = await connection.QueryAsync<SingleImageProductDto, SimpleProductImageDto, SimpleCategoryDto, SimpleProductPromotionDto, SingleImageProductDto>(query, (product, image, category, promotion) =>
                {
                    product.Category = category;
                    product.Image = image;
                    product.Promotion = promotion;
                    return product;
                }, param: new { Now = DateTime.Now });
                return products.ToList();
            }
        }

        public async Task UpdateProductSales(long productId, double sales)
        {
            Product product = await _dbContext.Product.FindAsync(productId) ?? throw new GeneralException(ApplicationConstant.NOT_FOUND, ApplicationConstant.NOT_FOUND_CODE);
            product.Sales += sales;
            _dbContext.Product.Update(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}
