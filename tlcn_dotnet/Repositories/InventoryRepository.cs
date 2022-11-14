using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Repositories
{
    public interface InventoryRepository
    {
        public Task<Inventory> GetInventoryById(long id);
        public Task<Inventory> AddInventory(AddInventoryDto addInventoryDto);
        public Task<IEnumerable<Inventory>> SearchInventory(string? keyword, long? productId, double? minQuantity,
            double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice, DateTime? fromDeliveryDate,
            DateTime? toDeliveryDate, DateTime? fromExpireDate, DateTime? toExpireDate,
            long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder, int page = 1);
    }
}
