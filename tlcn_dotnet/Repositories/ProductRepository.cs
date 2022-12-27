using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using tlcn_dotnet.Constant;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        private readonly DapperContext _dapperContext;
        public ProductRepository(MyDbContext dbContext, DapperContext dapperContext): base(dbContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<dynamic> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice,
            long? categoryId, ProductOrderBy? productOrderBy, SortOrder? sortOrder, int page, int pageSize)
        {
            IQueryable<Product> queryProduct = _dbContext.Product
                .Include(product => product.Category)
                .Include(product => product.ProductImages)
                .Include(product => product.Reviews)
                .Include(product => product.BillDetails)
                    .ThenInclude(bd => bd.Bill)
                    .ThenInclude(bill => bill.Cart);

            if (keyword != null)
                queryProduct = queryProduct.Where(product => product.Name.Contains(keyword) || product.Description.Contains(keyword));
            if (minPrice != null)
                queryProduct = queryProduct.Where(product => product.Price >= minPrice);
            if (maxPrice != null)
                queryProduct = queryProduct.Where(product => product.Price <= maxPrice);
            if (categoryId != null)
                queryProduct = queryProduct.Where(product => product.Category.Id == categoryId);

            if (productOrderBy == null || productOrderBy == ProductOrderBy.ID)
                queryProduct = sortOrder == SortOrder.ASC ? 
                    queryProduct.OrderBy(product => product.Id) : 
                    queryProduct.OrderByDescending(product => product.Id);
            else if(productOrderBy == ProductOrderBy.PRICE)
                queryProduct = sortOrder == SortOrder.ASC ?
                    queryProduct.OrderBy(product => product.Price) :
                    queryProduct.OrderByDescending(product => product.Price);

            IEnumerable<Product> products = await queryProduct.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
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
            string query = @"SELECT TOP 1 Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity,
										AVG(Review.Rating) as Rating,
										SUM(BillDetail.Quantity) as Sales,
			                            Image.Id, Image.Url, Image.FileName,
										Category.Id, Category.Name
                            FROM Product LEFT OUTER JOIN BillDetail ON Product.Id = BillDetail.ProductId
                            LEFT OUTER JOIN Bill ON Bill.Id = BillDetail.BillId AND Bill.PurchaseDate IS NOT NULL
							LEFT OUTER JOIN Category ON Product.CategoryId = Category.Id
							LEFT OUTER JOIN Review ON Review.ProductId = Product.Id
                            OUTER APPLY (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url FROM ProductImage where ProductImage.ProductId = Product.Id) as Image
                            GROUP BY Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity,
			                            Image.Id, Image.Url, Image.FileName, Category.Id, Category.Name
                            ORDER BY sum(
								CASE Product.MinPurchase
								WHEN 0 THEN BillDetail.Quantity / 1
								ELSE BillDetail.Quantity / Product.MinPurchase
								END
							) desc";
            using (var connection = _dapperContext.CreateConnection())
            {
                var products = await connection.QueryAsync<SingleImageProductDto, SimpleProductImageDto, SimpleCategoryDto, SingleImageProductDto>(query, (product, image, category) =>
                {
                    product.Category = category;
                    product.Image = image;
                    return product;
                });
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
            string query = @"SELECT TOP 8 Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity,
										AVG(Review.Rating) as Rating,
										SUM(CASE Product.MinPurchase
											WHEN 0 THEN BillDetail.Quantity / 1
											ELSE BillDetail.Quantity / Product.MinPurchase
											END) as Sales,
			                            Image.Id, Image.Url, Image.FileName,
										Category.Id, Category.Name
                            FROM Product LEFT OUTER JOIN BillDetail ON Product.Id = BillDetail.ProductId
                            LEFT OUTER JOIN Bill ON Bill.Id = BillDetail.BillId AND Bill.PurchaseDate IS NOT NULL
							LEFT OUTER JOIN Category ON Product.CategoryId = Category.Id
							LEFT OUTER JOIN Review ON Review.ProductId = Product.Id
                            OUTER APPLY (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url FROM ProductImage where ProductImage.ProductId = Product.Id) as Image
                            GROUP BY Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity,
			                            Image.Id, Image.Url, Image.FileName, Category.Id, Category.Name
                            ORDER BY sum(
								CASE Product.MinPurchase
								WHEN 0 THEN BillDetail.Quantity / 1
								ELSE BillDetail.Quantity / Product.MinPurchase
								END
							) desc";
            using (var connection = _dapperContext.CreateConnection())
            {
                var products = await connection.QueryAsync<SingleImageProductDto, SimpleProductImageDto, SimpleCategoryDto, SingleImageProductDto>(query, (product, image, category) =>
                {
                    product.Category = category;
                    product.Image = image;
                    return product;
                });
                return products.ToList();
            }
        }
    }
}
