using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.CartDto
{
    public class RequestFilterProcessCartDto
    {
        public string? KeyWordType { get; set; } // ONLY ACCEPT 2 VALUES: NAME OR PHONE, OTHERWISE TAKE NULL VALUE
        public string? KeyWord { get; set; } = string.Empty;
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? FromCreatedDate { get; set; }
        public string? ToCreatedDate { get; set; }
        public string? FromTotal { get; set; }
        public string? ToTotal { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public CartStatus? CartStatus { get; set; }
        public string? SortBy { get; set; } = "CREATEDDATE"; //ONLY ACCEPT 2 VALUES: CREATEDDATE OR TOTAL
        public string? Order { get; set; } = "ASC"; //ONLY ACCEPT 2 VALUES: ASC OR DESC
        public string? Page { get; set; } = "1";
        public string? PageSize { get; set; } = "5";
    }

}
