using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IAccountRepository: IGenericRepository<Account>
    {
        public Task<dynamic> GetAccount(string keyword, AccountKeywordType keywordType, string role, int page, int pageSize);
        public string GetVerifyTokenById(long id);
        public Task<Account> FindByEmail(string email);

    }
}
