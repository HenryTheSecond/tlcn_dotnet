using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("user/register")]
        public async Task<DataResponse> UserRegister([FromBody] RegisterAccountDto registerAccountDto)
        {
            return await _authService.RegisterAccount(registerAccountDto);
        }

        [HttpGet("confirm")]
        public async Task<DataResponse> ConfirmAccount(string token)
        {
            return await _authService.ConfirmAccount(token);
        }

        [HttpPost("login")]
        public async Task<DataResponse> Login([FromBody] LoginRequest loginRequest)
        {
            return await _authService.Login(loginRequest);
        }

        [HttpPost("{strRole}/register")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> EmployeeRegister([FromBody] RegisterEmployeeDto registerEmployeeDto, string strRole)
        {
            Role role;
            if (Enum.TryParse<Role>(strRole, out role))
            {
                return await _authService.RegisterAccount(registerEmployeeDto, role: role);
            }
            throw new GeneralException("ROLE NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
        }

        [HttpGet("admin/account")]
        public async Task<DataResponse> FilterAccount(string? keyword,
            [IsEnum(EnumType = typeof(AccountKeywordType), ErrorMessage = "KEY WORD TYPE IS INVALID")] string? keywordType = "NAME", 
            string? role = "", string? page = "1", string? pageSize = "10")
        {
            if (keyword == null)
                keyword = string.Empty;
            AccountKeywordType enumKeyWordType;
            Console.WriteLine(keywordType);
            if (Enum.TryParse<AccountKeywordType>(keywordType, true, out enumKeyWordType) == false)
                enumKeyWordType = AccountKeywordType.NAME;
            Console.WriteLine(enumKeyWordType.ToString()); Console.WriteLine(keyword == "");
            try
            {
                Enum.Parse<Role>(role, true);
            }
            catch (Exception e)
            {
                role = "";
            }

            int numberPage;
            int numberPageSize;
            Int32.TryParse(page, out numberPage);
            Int32.TryParse(pageSize, out numberPageSize);
            numberPage = numberPage == 0 ? 1 : numberPage;
            numberPageSize = numberPageSize > 0 ? numberPageSize : 10;

            return await _authService.GetAccount(keyword.Trim(), enumKeyWordType, role, numberPage, numberPageSize);
        }

        [HttpPost("changePassword")]
        public async Task<DataResponse> ChangePassword(ChangePasswordRequest changePasswordRequest)
        { 
            return await _authService.ChangePassword(changePasswordRequest);
        }

        [HttpGet("changePassword")]
        public async Task<DataResponse> ConfirmChangePassword(string token)
        { 
            return await _authService.ConfirmChangePassword(token);
        }

        [HttpGet("admin/account/{strId}")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> AdminGetAccountById(string strId)
        {
            long? id = Util.ParseId(strId) ?? throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _authService.GetAccountById(id.Value);
        }

        [HttpGet("profile")]
        [CustomAuthorize]
        public async Task<DataResponse> GetProfile([FromHeader(Name = "Authorization")] string authorization)
        {
            return await _authService.GetProfile(authorization);
        }

        [HttpPost("uploadPhoto")]
        [CustomAuthorize]
        public async Task<DataResponse> UploadPhoto([FromHeader(Name = "Authorization")] string authorization, [FromForm(Name = "photo")] IFormFile photo)
        {
            return await _authService.UploadPhoto(authorization, photo);
        }

        [HttpPut("updateProfile")]
        [CustomAuthorize]
        public async Task<DataResponse> UpdateProfile([FromHeader(Name = "Authorization")] string authorization, [FromBody] UpdateProfileRequest request)
        {
            return await _authService.UpdateProfile(authorization, request);
        }

        [HttpPut("updateRole")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> UpdateRole(UpdateRoleRequest request)
        {
            return await _authService.UpdateAccountRole(request);
        }

        [HttpGet("admin/manageEmployee")]
        public async Task<DataResponse> AdminManageEmployee([FromQuery] AdminManageEmployeeRequest request)
        {
            return await _authService.AdminManageEmployee(request);
        }

        [HttpGet("admin/manageUser")]
        public async Task<DataResponse> AdminManageUser([FromQuery] AdminManageUserRequest request)
        {
            return await _authService.AdminManageUser(request);
        }
    }
}
