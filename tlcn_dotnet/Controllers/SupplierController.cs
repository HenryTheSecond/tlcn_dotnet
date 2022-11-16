using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.SupplierDto;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        [HttpGet]
        public async Task<DataResponse> GetAllSupplier()
        {
            string a = null;
            Console.WriteLine(a == null);
            return await _supplierService.GetAllSupplier();
        }

        [HttpPost]
        public async Task<DataResponse> AddSupplier(AddSupplierDto addSupplierDto)
        {
            return await _supplierService.AddSupplier(addSupplierDto);
        }

        [HttpDelete("{strId}")]
        public async Task<DataResponse> DeleteSupplier(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null)
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _supplierService.DeleteSupplier(id);
        }

        [HttpPut("{strId}")]
        public async Task<DataResponse> EditSupplier(string strId, [FromBody] SimpleSupplierDto simpleSupplierDto)
        {
            long? id = Util.ParseId(strId);
            if (id == null)
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _supplierService.EditSupplier(id, simpleSupplierDto);
        }
    }
}
