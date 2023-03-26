using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Dto.GoogleAccountDto;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GoogleAccountController : ControllerBase
    {
        private readonly IGoogleAccountService _googleAccountService;
        public GoogleAccountController(IGoogleAccountService googleAccountService)
        {
            _googleAccountService = googleAccountService;
        }
        [HttpPost("login")]
        public async Task<DataResponse> LoginWithGoogle([FromBody] GoogleLoginRequest request)
        {
            return await _googleAccountService.LoginWithGoogle(request);
        }
    }
}
