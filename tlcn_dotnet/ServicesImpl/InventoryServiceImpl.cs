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
        public async Task<DataResponse> GetInventoryById(long id)
        {
            Inventory inventory = await _inventoryRepository.GetInventoryById(id);
            if (inventory == null)
                throw new GeneralException("INVENTORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(_mapper.Map<SimpleInventoryDto>(inventory));
        }
    }
}
