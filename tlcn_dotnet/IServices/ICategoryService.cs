using tlcn_dotnet.Dto.CategoryDto;

namespace tlcn_dotnet.Services
{
    public interface ICategoryService
    {
        public Task<DataResponse> GetAllCategory();
        public Task<DataResponse> AddCategory(SimpleCategoryDto category);
        public Task<DataResponse> DeleteCategory(long? id);
        public Task<DataResponse> EditCategory(long? id, SimpleCategoryDto simpleCategoryDto);
    }
}
