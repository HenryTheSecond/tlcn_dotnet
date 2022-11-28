using tlcn_dotnet.Dto.SupplierDto;

namespace tlcn_dotnet.Services
{
    public interface ISupplierService
    {
        public Task<DataResponse> GetAllSupplier();
        public Task<DataResponse> AddSupplier(AddSupplierDto addSupplierDto);
        public Task<DataResponse> DeleteSupplier(long? id);
        public Task<DataResponse> EditSupplier(long? id, SimpleSupplierDto simpleSupplierDto);
        public Task<DataResponse> GetAllSupplierIdAndName();
    }
}
