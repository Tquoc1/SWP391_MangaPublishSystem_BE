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
        private readonly ChapterRepository _chapterRepository;

        public DashboardService(
            SeriesRepository seriesRepository,
            MangakaAssistantRepository mangakaAssistantRepository,
            PageIssueRepository pageIssueRepository,
            UserRepository userRepository,
            ChapterRepository chapterRepository)
        {
            _seriesRepository = seriesRepository;
            _mangakaAssistantRepository = mangakaAssistantRepository;
            _pageIssueRepository = pageIssueRepository;
            _userRepository = userRepository;
            _chapterRepository = chapterRepository;
        }

        public async Task<List<SeriesDto>> GetTopSeriesAsync()
        {
            var seriesList = await _seriesRepository.GetAllWithDetailsAsync();
            
            return seriesList
                .Where(s => s.Isdeleted != true && (s.Status == "Approved" || s.Status == "Ongoing" || s.Status == "Completed"))
                .OrderByDescending(s => s.Createdat)
                .Take(5)
                .Select(s => new SeriesDto
                {
                    Seriesid = s.Seriesid,
                    Title = s.Title,
                    Coverimageurl = s.Coverimageurl,
                    Status = s.Status,
                    Createdat = s.Createdat
                }).ToList();
        }

        public async Task<DashboardDto.AdminOverviewResponse> GetAdminOverviewAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var series = await _seriesRepository.GetAllAsync();
            var chapters = await _chapterRepository.GetChaptersAsync();
            
            return new DashboardDto.AdminOverviewResponse
            {
                TotalUsers = users.Count,
                TotalMangakas = users.Count(u => u.Roleid == 4),
                TotalSeries = series.Count,
                TotalChapters = chapters.Count
            };
        }

        public async Task<DashboardDto.AdminSeriesStatsResponse> GetAdminSeriesStatsAsync()
        {
            var series = await _seriesRepository.GetAllAsync();
            
            int pending = series.Count(s => s.Status == "Pending");
            int approved = series.Count(s => s.Status == "Approved");
            int rejected = series.Count(s => s.Status == "Rejected");
            int ongoing = series.Count(s => s.Status == "Ongoing");
            int completed = series.Count(s => s.Status == "Completed");

            return new DashboardDto.AdminSeriesStatsResponse
            {
                PendingSeries = pending,
                ApprovedSeries = approved,
                RejectedSeries = rejected,
                OngoingSeries = ongoing,
                CompletedSeries = completed
            };
        }
    }
}
