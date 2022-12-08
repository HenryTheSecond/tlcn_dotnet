using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.BillDetailDto;

namespace tlcn_dotnet.Dto.BillDto
{
    public class BillWithBillDetailDto
    {
        public long? Id { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public Decimal? Total { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? OrderCode { get; set; }
        public IList<BillDetailWithProductDto> BillDetails { get; set; }
    }
}
