using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.SupplierDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class SupplierRepository :GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(MyDbContext dbContext): base(dbContext)
        {

        }
    }
}
