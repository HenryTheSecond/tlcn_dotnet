using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class LoginRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
