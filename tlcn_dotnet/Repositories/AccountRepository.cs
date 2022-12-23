using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class AccountRepository: GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(MyDbContext dbContext) : base(dbContext) { }

        public async Task<dynamic> GetAccount(string keyword, AccountKeywordType keywordType, string role, int page, int pageSize)
        {
            IQueryable<Account> queryAccount = _dbContext.Account;
            if (keywordType == AccountKeywordType.NAME)
            {
                queryAccount = queryAccount.Where(account => (account.LastName + " " + account.FirstName).Contains(keyword) || 
                        (account.FirstName + " " + account.LastName).Contains(keyword));
            }
            else if (keywordType == AccountKeywordType.EMAIL)
            {
                queryAccount = queryAccount.Where(account => account.Email.Contains(keyword));
            }
            else if (keywordType == AccountKeywordType.PHONE)
            {
                queryAccount = queryAccount.Where(account => account.Phone.Contains(keyword));
            }
            if (role.Trim() != "")
            {
                Role enumRole = Enum.Parse<Role>(role, true); //Cannot parse or use ToString() inside linq
                queryAccount = queryAccount
                   .Where(account => account.Role == enumRole);
            }
            var accounts = await queryAccount.OrderBy(account => account.FirstName)
                .OrderBy(account => account.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            long total = await queryAccount.LongCountAsync();
            return new
            {
                Accounts = accounts,
                Total = total
            };
        }

        public async Task<Account> FindByEmail(string email)
        {
            return await _dbContext.Account.Where(account => account.Email == email).SingleOrDefaultAsync();
        }

        public string GetVerifyTokenById(long id)
        {
            return  _dbContext.Account.Find(id).VerifyToken;
        }
    }
}
