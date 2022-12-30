using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<DataResponse> GetAllCategory()
        {
            return await _categoryService.GetAllCategory();
        }

        [HttpPost]
        public async Task<DataResponse> AddCategory([FromBody] SimpleCategoryDto simpleCategoryDto)
        {
            return await _categoryService.AddCategory(simpleCategoryDto);
        }

        [HttpDelete("{strId}")]
        public async Task<DataResponse> DeleteCategory(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _categoryService.DeleteCategory(id);
        }

        [HttpPut("{strId}")]
        public async Task<DataResponse> EditCategory(string strId, [FromBody] SimpleCategoryDto simpleCategoryDto)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _categoryService.EditCategory(id, simpleCategoryDto);
        }

        [HttpGet("{strId}")]
        public async Task<DataResponse> GetCategoryById(string strId)
        { 
            long? id = Util.ParseId(strId) ?? throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _categoryService.GetCategoryById(id.Value);
        }
    }
}
