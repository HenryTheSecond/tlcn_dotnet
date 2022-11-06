namespace tlcn_dotnet.Services
{
    public interface LocationService
    {
        public Task<DataResponse> GetAllCountryAndCity();
        public Task<DataResponse> GetVietnamLocation();
    }
}
