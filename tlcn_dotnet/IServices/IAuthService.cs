using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.AccountDto;

namespace tlcn_dotnet.Services
{
    public interface IAuthService
    {
        public Task<DataResponse> RegisterAccount(RegisterAccountDto registerAccountDto, Role role = Role.ROLE_USER);
        public Task<DataResponse> ConfirmAccount(string token);
        public Task<DataResponse> Login(LoginRequest loginRequest);
    }
}
