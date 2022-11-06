using System.Text.Json.Nodes;
using tlcn_dotnet.Services;
using tlcn_dotnet.ServicesImpl;

namespace tlcn_dotnet.Utils
{
    public class Util
    {
        private static readonly LocationService _locationService = new LocationServiceImpl();
        public static long? ParseId(string strId)
        {
            try
            {
                return long.Parse(strId);
            }
            catch (Exception e) when (e is FormatException || e is ArgumentNullException)
            {
                return null;
            }
        }

        //Return null if address is correct, otherwise return error message
        public static async Task<string> CheckVietnameseAddress(string cityId, string districtId, string wardId)
        {
            if (cityId == null || cityId.Trim() == "" || districtId == null || districtId.Trim() == "" || wardId == null || wardId == "")
                return "Address is invalid";
            JsonNode vietnameseLocation = (JsonNode)(await _locationService.GetVietnamLocation()).Data;
            foreach (JsonNode city in vietnameseLocation.AsArray())
            {
                if (cityId == city["level1_id"].ToString())
                {
                    foreach (JsonNode district in city["level2s"].AsArray())
                    {
                        if (districtId == district["level2_id"].ToString())
                        {
                            foreach (JsonNode ward in district["level3s"].AsArray())
                            {
                                if (wardId == ward["level3_id"].ToString())
                                {
                                    return null;
                                }
                            }
                            return "Ward is invalid";
                        }
                    }
                    return "District is invalid";
                }

            }
            return "City is invalid";
        }
    }
}
