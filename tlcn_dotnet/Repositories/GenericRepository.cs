using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MyDbContext _dbContext;
        public GenericRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Add(T entity)
        {
            T entityDb = (await _dbContext.Set<T>().AddAsync(entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return entityDb;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task Remove(long id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity == null)
                throw new GeneralException(ApplicationConstant.NOT_FOUND, ApplicationConstant.NOT_FOUND_CODE);
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) //throw exception if cannot find entity
            {
                throw new GeneralException("NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            }
            return entity;
        }
    }
}
