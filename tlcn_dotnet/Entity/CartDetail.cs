using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(ProductId), nameof(CartId), IsUnique = true)]
    public class CartDetail
    {
        [Key]
        public long? Id { get; set; }
        public CartDetailStatus Status { get; set; }
        public ProductUnit Unit { get; set; }
        public double Quantity { get; set; }

        public Product Product { get; set; }
        public long? ProductId { get; set; }
        public Cart Cart { get; set; }
        public long? CartId { get; set; }

        public Account Account { get; set; }
    }
}
