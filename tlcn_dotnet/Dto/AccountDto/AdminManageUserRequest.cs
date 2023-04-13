using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class AdminManageUserRequest
    {
        public ManageUserSearchBy? SearchBy { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
