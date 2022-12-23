using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.InventoryDto;

namespace tlcn_dotnet.Services
{
    public interface IInventoryService
    {
        public Task<DataResponse> GetInventoryById(long id);
        public Task<DataResponse> AddInventory(AddInventoryDto addInventoryDto);
        public Task<DataResponse> FilterInventory(string? keyword, long? productId, double? minQuantity,
            double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice, DateTime? fromDeliveryDate,
            DateTime? toDeliveryDate, DateTime? fromExpireDate, DateTime? toExpireDate,
            long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder, int page = 1, int pageSize = 8);
        public Task<DataResponse> EditInventory(long id, EditInventoryDto editInventoryDto);
    }
}
