using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Entity
{
    public class InventoryNotification
    {
        [Key]
        public long Id { get; set; }
        public string Content { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public InventoryNotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; }
    }
}
