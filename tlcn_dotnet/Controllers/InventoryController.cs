using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [HttpGet("{strId}")]
        public async Task<DataResponse> GetInventoryById(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralExceptio(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);

            return await _inventoryService.GetInventoryById(id.Value);
        }
    }
}
