using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.SupplierDto
{
    public class AddSupplierDto
    {
        [Required(ErrorMessage = "Supplier name is required")]
        public string Name { get; set; }
        public string? CountryCode { get; set; }
        public string? CityCode { get; set; }
        public string? DetailLocation { get; set; }
    }
}
