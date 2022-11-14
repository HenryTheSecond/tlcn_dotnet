using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Repositories
{
    public interface InventoryRepository
    {
        public Task<Inventory> GetInventoryById(long id);
    }
}
