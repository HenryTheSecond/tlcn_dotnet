using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Repositories;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.ServicesImpl
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper)
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

        public async Task<DataResponse> EditInventory(long id, EditInventoryDto editInventoryDto)
        {
            Inventory inventoryDb = await _inventoryRepository.UpdateInventory(id, editInventoryDto);
            return new DataResponse(_mapper.Map<SimpleInventoryDto>(inventoryDb));
        }

        public async Task<DataResponse> FilterInventory(string? keyword, long? productId, double? minQuantity,
            double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice,
            DateTime? fromDeliveryDate, DateTime? toDeliveryDate, DateTime? fromExpireDate,
            DateTime? toExpireDate, long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder,
            int page = 1, int pageSize = 8)
        {
            var result = await _inventoryRepository.SearchInventory(keyword, productId, minQuantity,
                    maxQuantity, minImportPrice, maxImportPrice, fromDeliveryDate, toDeliveryDate,
                    fromExpireDate, toExpireDate, supplierId, unit, inventoryOrderBy, sortOrder, page, pageSize);
            var inventories = _mapper.Map<IEnumerable<SimpleInventoryDto>>(result.inventories);
            return new DataResponse(new { 
                inventories = inventories,
                maxPage = Util.CalculateMaxPage(result.total, pageSize),
                currentPage = page
            });
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
