namespace tlcn_dotnet.Dto.AccountDto
{
    public class AccountReviewDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CityId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string PhotoUrl { get; set; }
    }
}
