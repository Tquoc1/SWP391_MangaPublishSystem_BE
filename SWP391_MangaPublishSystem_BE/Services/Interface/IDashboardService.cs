using DTOs;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IDashboardService
    {
        Task<DashboardDto.MangakaDashboardResponse> GetMangakaDashboardAsync(int mangakaId);
        Task<DashboardDto.AssistantDashboardResponse> GetAssistantDashboardAsync(int assistantId);
        Task<DashboardDto.AdminDashboardResponse> GetAdminDashboardAsync();
    }
}
