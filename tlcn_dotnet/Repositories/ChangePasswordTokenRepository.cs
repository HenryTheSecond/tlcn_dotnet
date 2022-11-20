using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class ChangePasswordTokenRepository: GenericRepository<ChangePasswordToken>, IChangePasswordTokenRepository
    {
        public ChangePasswordTokenRepository(MyDbContext dbContext) : base(dbContext) { }

        public async Task<ChangePasswordToken> FindByToken(string token)
        {
            return await _dbContext.ChangePasswordToken
                .Include(changePasswordToken => changePasswordToken.Account)
                .Where(changePasswordToken => changePasswordToken.Token == token)
                .SingleOrDefaultAsync();
        }
    }
}
