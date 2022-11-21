using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [HttpGet("{strId}")]
        public async Task<DataResponse> GetInventoryById(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);

            return await _inventoryService.GetInventoryById(id.Value);
        }

        [HttpPost]
        public async Task<DataResponse> AddInventory([FromBody] AddInventoryDto inventory)
        {
            return await _inventoryService.AddInventory(inventory);
        }

        [HttpGet]
        public async Task<DataResponse> FilterInventory(string? keyword, string? productId, string? minQuantity,
            string? maxQuantity, string? minImportPrice, string? maxImportPrice, string? fromDeliveryDate,
            string? toDeliveryDate, string? fromExpireDate, string? toExpireDate,
            string? supplierId, string? unit,  string? orderBy, 
            string? order, string? page = "1")
        {
            long? numberProductId = null, numberSupplierId = null;
            double? numberMinQuantity = null, numberMaxQuantity = null;
            decimal? numberMinImportPrice = null, numberMaxImportPrice = null;
            DateTime? dateFromDeliveryDate = null, dateToDeliveryDate = null;
            DateTime? dateFromExpireDate = null, dateToExpireDate = null;
            int? numberPage = 1;
            InventoryOrderBy inventoryOrderBy = InventoryOrderBy.DELIVERY_DATE;
            SortOrder sortOrder = SortOrder.DESC;
            try
            {
                numberProductId = Util.ConvertStringToDataType<long>(productId);
                numberSupplierId = Util.ConvertStringToDataType<long>(supplierId);
                numberMinQuantity = Util.ConvertStringToDataType<double>(minQuantity);
                numberMaxQuantity = Util.ConvertStringToDataType<double>(maxQuantity);
                numberMinImportPrice = Util.ConvertStringToDataType<decimal>(minImportPrice);
                numberMaxImportPrice = Util.ConvertStringToDataType<decimal>(maxImportPrice);
                dateFromDeliveryDate = Util.ConvertStringToDataType<DateTime>(fromDeliveryDate);
                dateToDeliveryDate = Util.ConvertStringToDataType<DateTime>(toDeliveryDate);
                dateFromExpireDate = Util.ConvertStringToDataType<DateTime>(fromExpireDate);
                dateToExpireDate = Util.ConvertStringToDataType<DateTime>(toExpireDate);

                if(unit != null && unit.Trim() != "")
                    Enum.Parse<ProductUnit>(unit);

                Enum.TryParse<InventoryOrderBy>(orderBy, out inventoryOrderBy);
                Enum.TryParse<SortOrder>(order, out sortOrder);

                numberPage = Util.ConvertStringToDataType<int>(page);
                numberPage = (numberPage == null && numberPage < 1) ? 1 : numberPage;
            }
            catch (Exception e) when (e is InvalidCastException || e is FormatException || e is ArgumentNullException)
            {
                throw new GeneralException(ApplicationConstant.BAD_REQUEST, ApplicationConstant.BAD_REQUEST_CODE);
            }


            return new DataResponse(await _inventoryService.FilterInventory(keyword, numberProductId,
                numberMinQuantity, numberMaxQuantity, numberMinImportPrice, numberMaxImportPrice, dateFromDeliveryDate,
                dateToDeliveryDate, dateFromExpireDate, dateToDeliveryDate, numberSupplierId, unit, inventoryOrderBy, sortOrder, numberPage.Value));
        }

        [HttpPut("{strId}")]
        public async Task<DataResponse> EditInventory(string strId, EditInventoryDto editInventoryDto)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _inventoryService.EditInventory(id.Value, editInventoryDto);
        }
    }
}
