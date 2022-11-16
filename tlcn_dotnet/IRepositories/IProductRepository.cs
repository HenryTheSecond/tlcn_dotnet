using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IProductRepository: IGenericRepository<Product>
    {
        public Task<dynamic> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice, long? categoryId, int page);
        public Task<Product> GetProductWithImageById(long? id);
    }
}
