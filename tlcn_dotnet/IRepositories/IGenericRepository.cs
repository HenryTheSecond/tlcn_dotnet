using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<T> Add(T entity);
        public Task Remove(long id);
        public Task<IEnumerable<T>> GetAll();
        public Task<T> Update(T entity);
        public Task<T> GetById(long id);
    }
}
