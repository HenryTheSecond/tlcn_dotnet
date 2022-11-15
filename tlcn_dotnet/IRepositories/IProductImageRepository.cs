using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IProductImageRepository: IGenericRepository<ProductImage>
    {
        public Task<List<ProductImage>> GetProductImageByProductId(long? productId);
    }
}
