using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("VietnamLocation")]
        public async Task<DataResponse> GetVietnamLocation()
        { 
            return await _locationService.GetVietnamLocation();
        }
        [HttpGet("GlobalLocation")]
        public async Task<DataResponse> GetGlobalLocation()
        {
            return await _locationService.GetAllCountryAndCity();
        }

        [HttpGet("city")]
        public async Task<DataResponse> GetAllVietnamCity()
        {
            return await _locationService.GetAllVietnamCity();
        }

        [HttpGet("district/{cityId}")]
        public async Task<DataResponse> GetAllDistrictByCityId(string cityId)
        {
            return await _locationService.GetAllDistrictByCityId(cityId);
        }

        [HttpGet("ward/{cityId}/{districtId}")]
        public async Task<DataResponse> GetAllWardByDistrictId(string cityId, string districtId)
        { 
            return await _locationService.GetAllWardByDistrictId(cityId, districtId);
        }
    }
}
