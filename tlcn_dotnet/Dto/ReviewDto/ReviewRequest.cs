using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Dto.ReviewDto
{
    public class ReviewRequest
    {
        public string Content { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }
    }
}
