using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Repositories;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class InventoryServiceImpl : InventoryService
    {
        private readonly InventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        public InventoryServiceImpl(InventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse> AddInventory(AddInventoryDto addInventoryDto)
        {
            return new DataResponse(
                _mapper.Map<SimpleInventoryDto>(
                    await _inventoryRepository.AddInventory(addInventoryDto)));
        }

        public async Task<DataResponse> FilterInventory(string? keyword, long? productId, double? minQuantity,
            double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice,
            DateTime? fromDeliveryDate, DateTime? toDeliveryDate, DateTime? fromExpireDate,
            DateTime? toExpireDate, long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder,
            int page = 1)
        {
            var inventories = _mapper.Map<IEnumerable<SimpleInventoryDto>>(
                    await _inventoryRepository.SearchInventory(keyword, productId, minQuantity,
                    maxQuantity, minImportPrice, maxImportPrice, fromDeliveryDate, toDeliveryDate,
                    fromExpireDate, toExpireDate, supplierId, unit, inventoryOrderBy, sortOrder, page));
            return new DataResponse(inventories);
        }

        public async Task<DataResponse> GetInventoryById(long id)
        {
            Inventory inventory = await _inventoryRepository.GetInventoryById(id);
            if (inventory == null)
                throw new GeneralException("INVENTORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(_mapper.Map<SimpleInventoryDto>(inventory));
        }
    }
}
