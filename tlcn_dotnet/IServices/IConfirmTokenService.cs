using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Services
{
    public interface IConfirmTokenService
    {
        public Task<ConfirmToken> CreateConfirmToken(Account account);
    }
}
