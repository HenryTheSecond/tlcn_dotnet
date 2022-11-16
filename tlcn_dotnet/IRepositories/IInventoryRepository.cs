using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Repositories
{
    public interface IInventoryRepository
    {
        public Task<Inventory> GetInventoryById(long id);
        public Task<Inventory> AddInventory(AddInventoryDto addInventoryDto);
        public Task<dynamic> SearchInventory(string? keyword, long? productId, double? minQuantity,
            double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice, DateTime? fromDeliveryDate,
            DateTime? toDeliveryDate, DateTime? fromExpireDate, DateTime? toExpireDate,
            long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder, int page = 1);
        public Task<Inventory> UpdateInventory(long id, EditInventoryDto editInventoryDto);
    }
}
