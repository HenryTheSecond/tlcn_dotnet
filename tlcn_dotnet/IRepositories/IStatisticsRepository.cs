using tlcn_dotnet.Dto.StatisticsDto;

namespace tlcn_dotnet.IRepositories
{
    public interface IStatisticsRepository
    {
        Task<IList<ProfitByAccount>> ProfitByAccount(DateTime? from, DateTime? to);
    }
}
