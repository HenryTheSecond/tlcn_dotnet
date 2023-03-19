using tlcn_dotnet.Dto.StatisticsDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface ICategoryRepository: IGenericRepository<Category>
    {
        Task<IList<CategoryStatisticsDto>> StatisticsByCategory(DateTime? from, DateTime? to);
    }
}
