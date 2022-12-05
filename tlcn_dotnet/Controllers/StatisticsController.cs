using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("countCart")]
        public Task<DataResponse> CountCart(string? fromDate, string? toDate)
        {
            return _statisticsService.CountCartByStatus(fromDate, toDate);
        }
    }
}
