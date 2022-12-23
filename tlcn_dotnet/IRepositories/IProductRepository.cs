﻿using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IProductRepository: IGenericRepository<Product>
    {
        public Task<dynamic> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice,
            long? categoryId, ProductOrderBy? productOrderBy, SortOrder? sortOrder, int page, int pageSize);
        public Task<Product> GetProductWithImageById(long? id);
        public Task<IList<Product>> GetTop8Product();
        public Task<Product> GetBestProduct();
        public Task<IList<Product>> GetAllProudctWithImage();
    }
}
