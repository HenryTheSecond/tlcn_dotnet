using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tlcn_dotnet.Entity
{
    public class Employee
    {
        [Key]
        [ForeignKey("Account")]
        public long? Id { get; set; }

        public Decimal? Salary { get; set; }
        public virtual Account Account { get; set; }
    }
}
