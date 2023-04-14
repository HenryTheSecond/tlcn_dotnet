using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class UpdateUserStatusRequest
    {
        public long UserId { get; set; }
        public UserStatus Status { get; set; }
    }
}
