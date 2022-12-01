using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.CartDto
{
    public class ProcessCartDto
    {
        public string? ProcessDescription { get; set; }
        public CartStatus Status { get; set; }
    }
}
