using System.Text.Json;
using System.Text.Json.Nodes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class LocationServiceImpl : LocationService
    {
        public async Task<DataResponse> GetAllCountryAndCity()
        {
            using (StreamReader r = new StreamReader(ApplicationConstant.COUNTRY_AND_CITY_DIRECTORY))
            {
                return new DataResponse(JsonSerializer.Deserialize<JsonNode>(r.ReadToEnd()));
            }
        }

        public async Task<DataResponse> GetVietnamLocation()
        {
            using (StreamReader r = new StreamReader(ApplicationConstant.VIETNAM_REGION))
            {
                return new DataResponse(JsonSerializer.Deserialize<JsonNode>(r.ReadToEnd())["data"]);
            }
        }
    }
}
