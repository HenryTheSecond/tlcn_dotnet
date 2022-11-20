using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "EMAIL IS MISSING")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "PASSWORD IS MISSING")]
        public string Password { get; set; }
    }
}
