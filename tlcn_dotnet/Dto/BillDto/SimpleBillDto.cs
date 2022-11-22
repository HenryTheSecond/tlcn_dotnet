using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.BillDto
{
    public class SimpleBillDto
    {
        public long? Id { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public Decimal? Total { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? OrderCode { get; set; } //Order code of Giao hang nhanh's API
    }
}
