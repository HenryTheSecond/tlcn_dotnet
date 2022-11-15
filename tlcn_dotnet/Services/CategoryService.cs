using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.Mapper;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<DataResponse> AddCategory(SimpleCategoryDto category)
        {
            Category newCategory = _mapper.Map<Category>(category);
            newCategory.Id = null;
            var dbCategory = await _categoryRepository.Add(newCategory);
            return new DataResponse(_mapper.Map<SimpleCategoryDto>(dbCategory));
        }

        public async Task<DataResponse> DeleteCategory(long? id)
        {
            await _categoryRepository.Remove(id.Value);
            return new DataResponse(true);

        }

        public async Task<DataResponse> EditCategory(long? id, SimpleCategoryDto simpleCategoryDto)
        {
            Category category = await _categoryRepository.GetById(id.Value);
            if (category == null)
                throw new GeneralException("CATEGORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            category.Name = (simpleCategoryDto.Name == null || simpleCategoryDto.Name.Trim() == "") ? category.Name : simpleCategoryDto.Name;
            return new DataResponse(
                _mapper.Map<SimpleCategoryDto>(await _categoryRepository.Update(category)));
        }

        public async Task<DataResponse> GetAllCategory()
        {
            return new DataResponse(
                _mapper.Map<IEnumerable<SimpleCategoryDto>>(
                    await _categoryRepository.GetAll()));
        }
    }
}
