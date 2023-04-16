namespace tlcn_dotnet.Dto.AccountDto
{
    public class AdminManageUserDetailRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
