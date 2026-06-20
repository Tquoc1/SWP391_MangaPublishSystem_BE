using DTOs;
using Repositories.Repository;
using Services.Interface;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Services.Implement
{
    public class DashboardService : IDashboardService
    {
        private readonly SeriesRepository _seriesRepository;
        private readonly MangakaAssistantRepository _mangakaAssistantRepository;
        private readonly PageIssueRepository _pageIssueRepository;
        private readonly UserRepository _userRepository;

        public DashboardService(
            SeriesRepository seriesRepository,
            MangakaAssistantRepository mangakaAssistantRepository,
            PageIssueRepository pageIssueRepository,
            UserRepository userRepository)
        {
            _seriesRepository = seriesRepository;
            _mangakaAssistantRepository = mangakaAssistantRepository;
            _pageIssueRepository = pageIssueRepository;
            _userRepository = userRepository;
        }

        public async Task<DashboardDto.MangakaDashboardResponse> GetMangakaDashboardAsync(int mangakaId)
        {
            var seriesList = await _seriesRepository.GetByMangakaIdAsync(mangakaId);
            
            var allContracts = await _mangakaAssistantRepository.GetAllAsync();
            var activeContracts = allContracts.Count(x => x.MangakaId == mangakaId && x.Status == "Accepted" && x.Isdeleted != true);
            
            var response = new DashboardDto.MangakaDashboardResponse
            {
                TotalSeries = seriesList.Count,
                ApprovedSeries = seriesList.Count(x => x.Status == "Approved"),
                PendingSeries = seriesList.Count(x => x.Status == "Pending"),
                ActiveContracts = activeContracts,
                RecentSeries = seriesList.OrderByDescending(x => x.Createdat).Take(5).Select(s => new SeriesDto
                {
                    Seriesid = s.Seriesid,
                    Title = s.Title,
                    Status = s.Status,
                    Createdat = s.Createdat
                }).ToList()
            };

            return response;
        }

        public async Task<DashboardDto.AssistantDashboardResponse> GetAssistantDashboardAsync(int assistantId)
        {
            var allContracts = await _mangakaAssistantRepository.GetAllAsync();
            var activeContracts = allContracts.Count(x => x.AssistantId == assistantId && x.Status == "Accepted" && x.Isdeleted != true);
            
            var response = new DashboardDto.AssistantDashboardResponse
            {
                ActiveContracts = activeContracts,
                PendingTasks = 0, 
                CompletedTasks = 0,
                RecentContracts = allContracts.Where(x => x.AssistantId == assistantId).OrderByDescending(x => x.Createdat).Take(5).Select(c => new MangakaAssistantDto
                {
                    ContractId = c.ContractId,
                    Status = c.Status,
                    Createdat = c.Createdat
                }).ToList()
            };

            return response;
        }

        public async Task<DashboardDto.AdminDashboardResponse> GetAdminDashboardAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var series = await _seriesRepository.GetAllAsync();
            
            return new DashboardDto.AdminDashboardResponse
            {
                TotalUsers = users.Count,
                TotalMangakas = users.Count(u => u.Roleid == 4),
                TotalAssistants = users.Count(u => u.Roleid == 5),
                TotalSeries = series.Count,
                PendingSeries = series.Count(s => s.Status == "Pending")
            };
        }
    }
}
