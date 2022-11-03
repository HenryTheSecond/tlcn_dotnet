using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Entity
{
    public class Review
    {
        [Key]
        public long? Id { get; set; }
        public string Content { get; set; }

        public virtual Account Account { get; set; }

        public double? Rating { get; set; }

        public DateTime PostDate { get; set; }
    }
}
