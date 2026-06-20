using System.Collections.Generic;

namespace DTOs
{
    public class DashboardDto
    {
        public class MangakaDashboardResponse
        {
            public int TotalSeries { get; set; }
            public int ApprovedSeries { get; set; }
            public int PendingSeries { get; set; }
            public int ActiveContracts { get; set; }
            public List<SeriesDto> RecentSeries { get; set; } = new();
            public List<PageIssueDto> PendingTasks { get; set; } = new();
        }

        public class AssistantDashboardResponse
        {
            public int ActiveContracts { get; set; }
            public int PendingTasks { get; set; }
            public int CompletedTasks { get; set; }
            public List<MangakaAssistantDto> RecentContracts { get; set; } = new();
            public List<PageIssueDto> RecentTasks { get; set; } = new();
        }
        
        public class AdminDashboardResponse
        {
            public int TotalUsers { get; set; }
            public int TotalMangakas { get; set; }
            public int TotalAssistants { get; set; }
            public int TotalSeries { get; set; }
            public int PendingSeries { get; set; }
        }
    }
}
