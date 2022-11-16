namespace tlcn_dotnet.Services
{
    public interface ILocationService
    {
        public Task<DataResponse> GetAllCountryAndCity();
        public Task<DataResponse> GetVietnamLocation();
    }
}
