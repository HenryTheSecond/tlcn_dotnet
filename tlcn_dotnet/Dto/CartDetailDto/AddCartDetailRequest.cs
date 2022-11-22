using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.CartDetailDto
{
    public class AddCartDetailRequest
    {
        public long ProductId { get; set; }
        [Range(0, Double.MaxValue)]
        public double Quantity { get; set; }
    }
}
