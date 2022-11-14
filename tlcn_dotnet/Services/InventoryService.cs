namespace tlcn_dotnet.Services
{
    public interface InventoryService
    {
        public Task<DataResponse> GetInventoryById(long id);
    }
}
