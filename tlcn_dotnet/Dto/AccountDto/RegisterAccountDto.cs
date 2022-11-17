using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class RegisterAccountDto
    {
        [MaxLength(12, ErrorMessage = "Phone number has more than 12 digits")]
        [MinLength(9, ErrorMessage = "Phone number has less than 9 digits")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string DetailLocation { get; set; }
    }
}
