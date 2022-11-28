using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.SupplierDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.ServicesImpl
{
    public class SupplierService : ISupplierService
    {
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepository;
        public SupplierService(IMapper mapper, ISupplierRepository supplierRepository)
        {
            _mapper = mapper;
            _supplierRepository = supplierRepository;
        }

        public async Task<DataResponse> AddSupplier(AddSupplierDto addSupplierDto)
        {
            string checkLocation = await Util.CheckGlobalCountryAndCity(addSupplierDto.CountryCode, addSupplierDto.CityCode);
            if (checkLocation != null)
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            Supplier supplierDb = await _supplierRepository.Add(_mapper.Map<Supplier>(addSupplierDto));
            return new DataResponse(_mapper.Map<SimpleSupplierDto>(supplierDb));
        }

        public async Task<DataResponse> DeleteSupplier(long? id)
        {
            await _supplierRepository.Remove(id.Value);
            return new DataResponse(true);
        }

        public async Task<DataResponse> GetAllSupplier()
        {
            var supplierList = await _supplierRepository.GetAll();
            return new DataResponse(supplierList);
        }

        public async Task<DataResponse> EditSupplier(long? id, SimpleSupplierDto simpleSupplierDto)
        {
            string checkLocation = await Util.CheckGlobalCountryAndCity(simpleSupplierDto.CountryCode, simpleSupplierDto.CityCode);
            if(checkLocation != null)
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            Supplier supplierDb = await _supplierRepository.GetById(id.Value);
            if(supplierDb == null)
                throw new GeneralException("SUPPLIER NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            supplierDb.Name = (simpleSupplierDto.Name != null && simpleSupplierDto.Name.Trim() != "") ? 
                simpleSupplierDto.Name : supplierDb.Name;
            supplierDb.CountryCode = simpleSupplierDto.CountryCode != null ? 
                simpleSupplierDto.CountryCode : supplierDb.CountryCode;
            supplierDb.CityCode = simpleSupplierDto.CityCode != null ?
                simpleSupplierDto.CityCode : supplierDb.CityCode;
            supplierDb.DetailLocation = simpleSupplierDto.DetailLocation != null ?
                simpleSupplierDto.DetailLocation : supplierDb.DetailLocation;
            supplierDb = await _supplierRepository.Update(supplierDb);
            return new DataResponse(_mapper.Map<SimpleSupplierDto>(supplierDb));
        }

        public async Task<DataResponse> GetAllSupplierIdAndName()
        {
            IEnumerable<Supplier> suppliers = await _supplierRepository.GetAll();
            return new DataResponse(_mapper.Map<IEnumerable<SupplierIdAndName>>(suppliers));
        }
    }
}
