namespace tlcn_dotnet.Dto.SupplierDto
{
    public class SimpleSupplierDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }

        public string? CountryCode { get; set; }
        public string? CityCode { get; set; }
        public string? DetailLocation { get; set; }
    }
}
