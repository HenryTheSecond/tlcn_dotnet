﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CategoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Mapper;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class CategoryServiceImpl : CategoryService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public CategoryServiceImpl(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<DataResponse> AddCategory(SimpleCategoryDto category)
        {
            Category newCategory = _mapper.Map<Category>(category);
            newCategory.Id = null;
            var dbCategory = _dbContext.Category.Add(newCategory).Entity;
            _dbContext.SaveChanges();
            return new DataResponse(_mapper.Map<SimpleCategoryDto>(dbCategory));
        }

        public async Task<DataResponse> DeleteCategory(long? id)
        {
            Category categoryDb = _dbContext.Category.Find(id);
            if (categoryDb == null)
                throw new GeneralException("Category not found", ApplicationConstant.NOT_FOUND_CODE);
            _dbContext.Category.Remove(categoryDb);
            _dbContext.SaveChangesAsync();
            return new DataResponse(true);
        }

        public async Task<DataResponse> EditCategory(long? id, SimpleCategoryDto simpleCategoryDto)
        {
            Category categoryDb = _dbContext.Category.Find(id);
            if (categoryDb == null)
                throw new GeneralException("Category not found", ApplicationConstant.NOT_FOUND_CODE);
            categoryDb.Name = simpleCategoryDto.Name == null? categoryDb.Name: simpleCategoryDto.Name;
            _dbContext.SaveChangesAsync();
            return new DataResponse(_mapper.Map<SimpleCategoryDto>(categoryDb));
        }

        public async Task<DataResponse> GetAllCategory()
        {
            return new DataResponse(_dbContext.Category.Select(c => _mapper.Map<SimpleCategoryDto>(c)));
        }
    }
}
