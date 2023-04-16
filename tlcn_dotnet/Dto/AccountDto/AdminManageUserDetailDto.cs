using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class AdminManageUserDetailDto: AccountResponse
    {

    }

    public class UserListCartDto
    { 
        public IList<UserCartDto> Carts { get; set; }
        public long Count { get; set; }
        public long MaxPage { get; set; }
        public int CurrentPage { get; set; }
        public decimal Total { get; set; }
    }

    public class UserCartDto
    {
        public long Id { get; set; }
        public string DetailLocation { get; set; }
        public CartStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProcessDescription { get; set; }
    }
}
