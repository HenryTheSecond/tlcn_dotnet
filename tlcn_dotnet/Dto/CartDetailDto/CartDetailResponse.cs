using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.GiftCartDto;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.CartDetailDto
{
    public class CartDetailResponse
    {
        public long Id { get; set; }
        public CartDetailStatus Status { get; set; }
        public ProductUnit Unit { get; set; }
        public double Quantity { get; set; }
        public decimal? Price { get; set; }
        public SingleImageProductDto Product { get; set; }
        public GiftCartResponse GiftCart { get; set; }
    }
}
