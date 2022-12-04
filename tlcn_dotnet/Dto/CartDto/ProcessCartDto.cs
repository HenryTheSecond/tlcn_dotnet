using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.CartDto
{
    public class ProcessCartDto
    {
        public string? ProcessDescription { get; set; }
        public CartStatus Status { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
