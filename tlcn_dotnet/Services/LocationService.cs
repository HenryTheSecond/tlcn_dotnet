using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class LocationService : ILocationService
    {
        public async Task<DataResponse> GetAllCountryAndCity()
        {
            using (StreamReader r = new StreamReader(ApplicationConstant.COUNTRY_AND_CITY_DIRECTORY))
            {
                return new DataResponse(JsonSerializer.Deserialize<JsonNode>(r.ReadToEnd()));
            }
        }

        public async Task<DataResponse> GetAllDistrictByCityId(string cityId)
        {
            using (StreamReader reader = new StreamReader(ApplicationConstant.VIETNAM_REGION))
            {
                var json = JsonSerializer.Deserialize<JsonNode>(reader.ReadToEnd())["data"];
                IList<dynamic> list = new List<dynamic>();
                foreach (var item in json.AsArray())
                {
                    if (item["level1_id"].GetValue<string>() == cityId)
                    {
                        foreach (var district in item["level2s"].AsArray())
                        {
                            list.Add(new
                            {
                                level2_id = district["level2_id"],
                                name = district["name"],
                                type = district["type"]
                            });
                        }
                        break;
                    }
                }
                return new DataResponse(list);
            }
        }

        public async Task<DataResponse> GetAllVietnamCity()
        {
            using (StreamReader reader = new StreamReader(ApplicationConstant.VIETNAM_REGION))
            {
                var json = JsonSerializer.Deserialize<JsonNode>(reader.ReadToEnd())["data"];
                IList<dynamic> list = new List<dynamic>();
                foreach (var item in json.AsArray())
                {
                    list.Add(new
                    {
                        level1_id = item["level1_id"],
                        name = item["name"],
                        type = item["type"]
                    });
                }
                return new DataResponse(list);
            }
        }

        public async Task<DataResponse> GetAllWardByDistrictId(string cityId, string districtId)
        {
            using (StreamReader reader = new StreamReader(ApplicationConstant.VIETNAM_REGION))
            {
                var json = JsonSerializer.Deserialize<JsonNode>(reader.ReadToEnd())["data"];
                IList<dynamic> list = new List<dynamic>();
                foreach (var item in json.AsArray())
                {
                    if (item["level1_id"].GetValue<string>() == cityId)
                    {
                        foreach (var district in item["level2s"].AsArray())
                        {
                            if (district["level2_id"].GetValue<string>() == districtId)
                            {
                                foreach (var ward in district["level3s"].AsArray())
                                {
                                    list.Add(new
                                    {
                                        level3_id = ward["level3_id"],
                                        name = ward["name"],
                                        type = ward["type"]
                                    });
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                return new DataResponse(list);
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
