﻿using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(MyDbContext dbContext): base(dbContext)
        {

        }

        public async Task<dynamic> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice, long? categoryId, int page)
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

            IEnumerable<Product> products = await queryProduct.Skip((page - 1) * 2).Take(2).ToListAsync();
            long total = await queryProduct.LongCountAsync();
            return new
            {
                Products = products,
                Total = total
            };
        }

        public async Task<Product> GetProductWithImageById(long? id)
        {
            return await _dbContext.Product.Include(product => product.ProductImages)
                .Include(product => product.Category)
                .FirstOrDefaultAsync(product => product.Id == id);
        }
    }
}