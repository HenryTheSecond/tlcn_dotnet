using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.AccountDto;

namespace tlcn_dotnet.Services
{
    public interface IAuthService
    {
        public Task<DataResponse> RegisterAccount(RegisterAccountDto registerAccountDto, Role role = Role.ROLE_USER);
        public Task<DataResponse> ConfirmAccount(string token);
        public Task<DataResponse> Login(LoginRequest loginRequest);
        public Task<DataResponse> GetAccount(string keyword, AccountKeywordType keywordType, string role, int page, int pageSize);
        public Task<DataResponse> ChangePassword(ChangePasswordRequest changePasswordRequest);
        public Task<DataResponse> ConfirmChangePassword(string token);
        public Task<DataResponse> GetAccountById(long id);
        public Task<DataResponse> GetProfile(string authorization);
        public Task<DataResponse> UploadPhoto(string authorization, IFormFile photo);
        public Task<DataResponse> UpdateProfile(string authorization, UpdateProfileRequest request);
        public Task<DataResponse> UpdateAccountRole(UpdateRoleRequest request);
        public Task<DataResponse> AdminManageEmployee(AdminManageEmployeeRequest request);
        public Task<DataResponse> AdminManageUser(AdminManageUserRequest request);
        public Task<DataResponse> AdminUpdateUserStatus(UpdateUserStatusRequest request);
    }
}
