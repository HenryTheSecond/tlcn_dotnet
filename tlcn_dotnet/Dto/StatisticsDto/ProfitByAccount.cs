using tlcn_dotnet.Dto.AccountDto;

namespace tlcn_dotnet.Dto.StatisticsDto
{
    public class ProfitByAccount
    {
        public AccountResponse Account { get; set; }
        public decimal Profit { get; set; }
    }
}
