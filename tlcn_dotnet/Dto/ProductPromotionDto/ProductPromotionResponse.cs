using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.ProductPromotionDto
{
    public class ProductPromotionResponse
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public ProductPromotionType Type { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; }
        public bool IsEnable { get; set; } = true;
    }
}
