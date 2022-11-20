using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IChangePasswordTokenRepository: IGenericRepository<ChangePasswordToken>
    {
        public Task<ChangePasswordToken> FindByToken(string token);
    }
}
