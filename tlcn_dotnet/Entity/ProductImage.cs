using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    public class ProductImage
    {
        [Key]
        public long? Id { get; set; }

        public string Url { get; set; }

        public string FileName { get; set; }

        public virtual Product? Product { get; set; }
    }
}
