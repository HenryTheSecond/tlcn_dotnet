namespace tlcn_dotnet.Dto.AccountDto
{
    public class AccountWithProviderResponse: AccountResponse
    {
        public string Provider { get; set; } = "System";
    }
}
