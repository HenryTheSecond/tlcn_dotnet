using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(AccountId), IsUnique = true)]
    public class CountCartNotificationByUser
    {
        [Key]
        public long Id { get; set; }
        public Account Account { get; set; }
        public long AccountId { get; set; }
        public long Count { get; set; } = 0;
    }
}
