using tlcn_dotnet.Dto.GoogleAccountDto;

namespace tlcn_dotnet.IServices
{
    public interface IGoogleAccountService
    {
        Task<DataResponse> LoginWithGoogle(GoogleLoginRequest request);
    }
}
