using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class ProductImageRepository: GenericRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(MyDbContext dbContext):base(dbContext)
        {

        }

        public async Task<List<ProductImage>> GetProductImageByProductId(long? productId)
        {
            return await _dbContext.ProductImage.Where(image => image.Product.Id == productId).ToListAsync();
        }
    }
}
