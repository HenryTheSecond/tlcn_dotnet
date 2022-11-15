using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class CategoryRepository :GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MyDbContext dbContext): base(dbContext) { }
    }
}
