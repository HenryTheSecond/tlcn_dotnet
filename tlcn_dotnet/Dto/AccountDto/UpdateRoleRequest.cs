using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Role Role { get; set; }

        public decimal? Salary { get; set; }
    }
}
