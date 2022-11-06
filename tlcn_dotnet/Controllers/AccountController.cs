using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
               _authService = authService;
        }

        [HttpGet]
        public async Task<DataResponse> test()
        {
            return await _authService.CreateToken();
        }

        [HttpPost("user/register")]
        public async Task<DataResponse> UserRegister([FromBody] RegisterAccountDto registerAccountDto)
        { 
            return await _authService.RegisterUser(registerAccountDto);
        }

        [HttpGet("confirm")]
        public async Task<DataResponse> ConfirmAccount(string token)
        {
            return await _authService.ConfirmAccount(token);
        }
    }
}
