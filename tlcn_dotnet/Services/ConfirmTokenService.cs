using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Services
{
    public interface ConfirmTokenService
    {
        public Task<ConfirmToken> CreateConfirmToken(Account account);
    }
}
