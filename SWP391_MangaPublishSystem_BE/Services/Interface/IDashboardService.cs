using DTOs;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IDashboardService
    {
        Task<List<SeriesDto>> GetTopSeriesAsync();
        Task<DashboardDto.AdminOverviewResponse> GetAdminOverviewAsync();
        Task<DashboardDto.AdminSeriesStatsResponse> GetAdminSeriesStatsAsync();
    }
}
