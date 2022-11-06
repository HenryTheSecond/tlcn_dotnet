using tlcn_dotnet.Dto.AccountDto;

namespace tlcn_dotnet.Services
{
    public interface AuthService
    {
        public Task<DataResponse> CreateToken();
        public Task<DataResponse> RegisterUser(RegisterAccountDto registerAccountDto);
        public Task<DataResponse> ConfirmAccount(string token);
    }
}
