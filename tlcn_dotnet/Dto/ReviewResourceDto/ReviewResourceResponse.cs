using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.ReviewResponseDto
{
    public class ReviewResourceResponse
    {
        public long Id { get; set; }
        public long ReviewId { get; set; }
        public ReviewResourceType Type { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
    }
}
