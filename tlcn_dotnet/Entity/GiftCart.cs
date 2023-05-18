namespace tlcn_dotnet.Entity
{
    public class GiftCart
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public Account Account { get; set; }
        public long AccountId { get; set; }
    }
}
