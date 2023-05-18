using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    //[Index(nameof(ProductId), nameof(CartId), IsUnique = true)]
    public class CartDetail
    {
        [Key]
        public long? Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public CartDetailStatus Status { get; set; }
        [Column(TypeName = "varchar(50)")]
        public ProductUnit Unit { get; set; }
        public double Quantity { get; set; }
        public decimal? Price { get; set; }
        public Product Product { get; set; }
        public long? ProductId { get; set; }
        public Cart Cart { get; set; }
        public long? CartId { get; set; }

        public Account Account { get; set; }
        public long AccountId { get; set; }
        public GiftCart? GiftCart { get; set; }
        public long? GiftCartId { get; set; }
    }
}
