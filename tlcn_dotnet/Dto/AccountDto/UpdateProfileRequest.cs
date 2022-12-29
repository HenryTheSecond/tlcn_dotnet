using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class UpdateProfileRequest
    {
        [MaxLength(12, ErrorMessage = "Phone number has more than 12 digits")]
        [MinLength(9, ErrorMessage = "Phone number has less than 9 digits")]
        public string? Phone { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? DetailLocation { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }
}
