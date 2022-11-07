using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.SupplierDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.ServicesImpl
{
    public class SupplierServiceImpl : SupplierService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        public SupplierServiceImpl(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<DataResponse> AddSupplier(AddSupplierDto addSupplierDto)
        {
            string checkLocation = await Util.CheckGlobalCountryAndCity(addSupplierDto.CountryCode, addSupplierDto.CityCode);
            if (checkLocation != null)
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            Supplier supplierDb = (await _dbContext.Supplier.AddAsync(_mapper.Map<Supplier>(addSupplierDto))).Entity;
            await _dbContext.SaveChangesAsync();
            return new DataResponse(_mapper.Map<SimpleSupplierDto>(supplierDb));
        }

        public async Task<DataResponse> DeleteSupplier(long? id)
        {
            try
            {
                _dbContext.Remove(_dbContext.Supplier.Single(supplier => supplier.Id == id));
                await _dbContext.SaveChangesAsync();
                return new DataResponse(true);
            }
            catch (InvalidOperationException e) //Single method throws this exception
            {
                throw new GeneralException("SUPPLIER NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            }
        }

        public async Task<DataResponse> GetAllSupplier()
        {
            var supplierList = _dbContext.Supplier.Select(supplier => _mapper.Map<SimpleSupplierDto>(supplier));
            return new DataResponse(supplierList);
        }

        public async Task<DataResponse> EditSupplier(long? id, SimpleSupplierDto simpleSupplierDto)
        {
            string checkLocation = await Util.CheckGlobalCountryAndCity(simpleSupplierDto.CountryCode, simpleSupplierDto.CityCode);
            if(checkLocation != null)
                throw new GeneralException(checkLocation, ApplicationConstant.BAD_REQUEST_CODE);
            Supplier supplierDb = await _dbContext.Supplier.FindAsync(id);
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
            await _dbContext.SaveChangesAsync();
            return new DataResponse(_mapper.Map<SimpleSupplierDto>(supplierDb));
        }
    }
}
