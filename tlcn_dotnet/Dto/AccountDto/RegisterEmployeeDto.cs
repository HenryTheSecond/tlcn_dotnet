namespace tlcn_dotnet.Dto.AccountDto
{
    public class RegisterEmployeeDto: RegisterAccountDto
    {
        public decimal? Salary { get; set; }
    }
}
