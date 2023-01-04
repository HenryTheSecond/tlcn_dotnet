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

        public async Task<bool> CheckAccountBuyItem(long accountId, long productId)
        {
            int count = _dbContext.CartDetail.Include(cd => cd.Product)
                .Include(cd => cd.Cart)
                .Include(cd => cd.Account)
                .Where(cd => cd.Account.Id == accountId && cd.Product.Id == productId && cd.Cart.Status == CartStatus.DELIVERIED).Count();
            return count > 0;
        }

        public async Task<dynamic> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice,
            long? categoryId, ProductOrderBy? productOrderBy, SortOrder? sortOrder, int page, int pageSize)
        {
            IQueryable<Product> queryProduct = _dbContext.Product
                .Include(product => product.Category)
                .Include(product => product.ProductImages)
                .Include(product => product.Reviews)
                .AsSplitQuery()
                .Include(product => product.BillDetails)
                    .ThenInclude(bd => bd.Bill)
                    .ThenInclude(bill => bill.Cart)
                    .AsSplitQuery();

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
										SUM(CASE Cart.Status
											WHEN 'DELIVERIED' THEN 
																CASE Product.MinPurchase
																WHEN 0 THEN BillDetail.Quantity / 1
																ELSE BillDetail.Quantity / Product.MinPurchase
																END
											ELSE 0
											END) as Sales,
			                            Image.Id, Image.Url, Image.FileName,
										Category.Id, Category.Name
                            FROM Product LEFT OUTER JOIN BillDetail ON Product.Id = BillDetail.ProductId
                            LEFT OUTER JOIN Bill ON Bill.Id = BillDetail.BillId
							LEFT OUTER JOIN Cart ON Cart.BillId = Bill.Id
							LEFT OUTER JOIN Category ON Product.CategoryId = Category.Id
							LEFT OUTER JOIN Review ON Review.ProductId = Product.Id
                            OUTER APPLY (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url FROM ProductImage where ProductImage.ProductId = Product.Id) as Image
                            GROUP BY Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity,
			                            Image.Id, Image.Url, Image.FileName, Category.Id, Category.Name
                            ORDER BY Sales desc";
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
										SUM(CASE Cart.Status
											WHEN 'DELIVERIED' THEN 
																CASE Product.MinPurchase
																WHEN 0 THEN BillDetail.Quantity / 1
																ELSE BillDetail.Quantity / Product.MinPurchase
																END
											ELSE 0
											END) as Sales,
			                            Image.Id, Image.Url, Image.FileName,
										Category.Id, Category.Name
                            FROM Product LEFT OUTER JOIN BillDetail ON Product.Id = BillDetail.ProductId
                            LEFT OUTER JOIN Bill ON Bill.Id = BillDetail.BillId
							LEFT OUTER JOIN Cart ON Cart.BillId = Bill.Id
							LEFT OUTER JOIN Category ON Product.CategoryId = Category.Id
							LEFT OUTER JOIN Review ON Review.ProductId = Product.Id
                            OUTER APPLY (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url FROM ProductImage where ProductImage.ProductId = Product.Id) as Image
                            GROUP BY Product.Id, Product.Name, Product.Price, Product.MinPurchase, Product.Status, Product.Unit, Product.Quantity,
			                            Image.Id, Image.Url, Image.FileName, Category.Id, Category.Name
                            ORDER BY Sales desc";
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
