using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.CartDto
{
    public class ProcessCartDto
    {
        public string? ProcessDescription { get; set; }
        public CartStatus Status { get; set; }
        [Range(1, double.PositiveInfinity)]
        public int Weight { get; set; }
        [Range(1, double.PositiveInfinity)]
        public int Length { get; set; }
        [Range(1, double.PositiveInfinity)]
        public int Width { get; set; }
        [Range(1, double.PositiveInfinity)]
        public int Height { get; set; }
    }
}
