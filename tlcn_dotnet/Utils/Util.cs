﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.LocationDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;
using tlcn_dotnet.ServicesImpl;

namespace tlcn_dotnet.Utils
{
    public class Util
    {
        private static readonly ILocationService _locationService = new LocationService();
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
                return null;
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

        //Return null if address is correct, other wise return error message
        public static async Task<string> CheckGlobalCountryAndCity(string countryCode, string cityCode)
        {
            if (countryCode == null || countryCode.Trim() == "" || cityCode == null || cityCode.Trim() == "")
                return null;
            JsonNode countries = (JsonNode)(await _locationService.GetAllCountryAndCity()).Data;
            foreach (JsonNode country in countries.AsArray())
            {
                if (countryCode == country["countryShortCode"].ToString())
                {
                    foreach (JsonNode city in country["regions"].AsArray())
                    {
                        if (cityCode == city["shortCode"].ToString())
                            return null;
                    }
                    return "City Code is invalid";
                }
            }
            return "Country Code is invalid";
        }

        public static async Task<VietnamLocationDto> FindVietnamLocation(string cityId, string districtId, string wardId)
        {
            VietnamLocationDto location = new VietnamLocationDto();
            JsonNode vietnameseLocation = (JsonNode)(await _locationService.GetVietnamLocation()).Data;
            foreach (JsonNode city in vietnameseLocation.AsArray())
            {
                if (cityId == city["level1_id"].ToString())
                {
                    location.City = city["name"].ToString();
                    foreach (JsonNode district in city["level2s"].AsArray())
                    {
                        if (districtId == district["level2_id"].ToString())
                        {
                            location.District = district["name"].ToString();
                            foreach (JsonNode ward in district["level3s"].AsArray())
                            {
                                if (wardId == ward["level3_id"].ToString())
                                {
                                    location.Ward = ward["name"].ToString();
                                    return location;
                                }
                            }
                        }
                    }
                }

            }
            return null;
        }

        public static long CalculateMaxPage(long quantity, int pageSize)
        {
            long maxPage = quantity / pageSize;
            long remainingItem = quantity % pageSize;
            if (remainingItem > 0)
                maxPage++;
            return maxPage;
        }

        public static T? ConvertStringToDataType<T>(string source, T? defaultValue = null) where T: struct
        {
            if (source == null || source.Trim() == "")
                return defaultValue;
            if (typeof(T).IsEnum)
                return Enum.Parse<T>(source, true);
            return (T?)Convert.ChangeType(source, typeof(T));

        }

        public static bool TryConvertStringToDataType<T>(string source, out T? dest, T? defaultValue = null) where T : struct
        {
            try
            {
                if (source == null || source.Trim() == "")
                {
                    dest = defaultValue;
                    return true;
                }
                if (typeof(T).IsEnum)
                {
                    dest = Enum.Parse<T>(source, true);
                    return true;
                }
                dest = (T?)Convert.ChangeType(source, typeof(T));
                return true;
            }
            catch(Exception e) when(e is InvalidCastException || e is FormatException || e is ArgumentNullException || e is ArgumentException)
            {
                dest = null;
                return false;
            }
        }

        public static JwtSecurityToken ReadJwtToken(string authorization)
        {
            var jwtToken = authorization.Substring(0, 7).ToLower() == "bearer " ?
                authorization.ToString().Substring("bearer ".Length) : authorization;
            var tokenValidator = new JwtSecurityTokenHandler();
            return tokenValidator.ReadJwtToken(jwtToken);
        }

        public static long ReadJwtTokenAndGetAccountId(string authorization)
        { 
            JwtSecurityToken jwtToken = ReadJwtToken(authorization);
            object accountId;
            jwtToken.Payload.TryGetValue("userId", out accountId);
            return Convert.ToInt64(accountId);
        }

        public static Account ReadJwtTokenAndParseToAccount(string authorization)
        {
            Account account = new Account();
            JwtSecurityToken jwtToken = ReadJwtToken(authorization);

            account.Id = Convert.ToInt64(jwtToken.Payload["userId"]);
            account.Email = (string)jwtToken.Payload[ClaimTypes.Email];
            account.Role = Enum.Parse<Role>(jwtToken.Payload[ClaimTypes.Role].ToString());
            account.Phone = (string?)jwtToken.Payload[ClaimTypes.MobilePhone];
            account.Status = Enum.Parse<UserStatus>(jwtToken.Payload["status"].ToString());
            account.CityId = (string?)jwtToken.Payload["cityId"];
            account.DistrictId = (string?)jwtToken.Payload["districtId"];
            account.WardId = (string?)jwtToken.Payload["wardId"];
            account.DetailLocation = (string?)jwtToken.Payload["detailLocation"];
            account.VerifyToken = (string?)jwtToken.Payload["verifyToken"];
            account.FirstName = (string)jwtToken.Payload["firstName"];
            account.LastName = (string)jwtToken.Payload["lastName"];
            account.PhotoUrl = (string)jwtToken.Payload["photoUrl"];

            return account;
        }

        public static string ComputeHMACSHA256(string secret, string message, HashFormat format = HashFormat.HEX, bool isLowerCase = true)
        {
            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(message));

            string result = string.Empty;
            if (format == HashFormat.HEX)
                result = Convert.ToHexString(hash);
            else if (format == HashFormat.BASE64)
                result = Convert.ToBase64String(hash);
            if (isLowerCase)
                result = result.ToLower();
            else
                result = result.ToUpper();
            return result;
        }

        public static decimal CalculatePrice(decimal price, ProductPromotion promotion)
        {
            if(promotion != null)
            {
                switch(promotion.Type)
                {
                    case ProductPromotionType.PRICE:
                        return price - promotion.Value;
                    case ProductPromotionType.PERCENTAGE:
                        return price * (100 - promotion.Value) / 100;
                    default:
                        return price;
                }
            }
            return price;
        }

        public static DateTime ConvertTimestampToDateTime(long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }
    }
}
