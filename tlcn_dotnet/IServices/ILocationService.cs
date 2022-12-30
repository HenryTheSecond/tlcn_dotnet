namespace tlcn_dotnet.Services
{
    public interface ILocationService
    {
        public Task<DataResponse> GetAllCountryAndCity();
        public Task<DataResponse> GetVietnamLocation();
        public Task<DataResponse> GetAllVietnamCity();
        public Task<DataResponse> GetAllDistrictByCityId(string cityId);
        public Task<DataResponse> GetAllWardByDistrictId(string cityId, string districtId);
    }
}
