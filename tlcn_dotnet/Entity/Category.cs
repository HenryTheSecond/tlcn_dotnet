using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tlcn_dotnet.Entity
{
    [Index(nameof(Category.Name), IsUnique = true)]
    public class Category
    {
        [Key]
        public long? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Product> products { get; set; }
    }
}
