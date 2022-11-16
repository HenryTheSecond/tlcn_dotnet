using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Services;

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
        public async Task<DataResponse> EmployeeRegister([FromBody] RegisterAccountDto registerAccountDto, string strRole)
        {
            Role role;
            if (Enum.TryParse<Role>(strRole, out role))
            {
                return await _authService.RegisterAccount(registerAccountDto, role: role);
            }
            throw new GeneralException("ROLE NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
        }

    }
}
