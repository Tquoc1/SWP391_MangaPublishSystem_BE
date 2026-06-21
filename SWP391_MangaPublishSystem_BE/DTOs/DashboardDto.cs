using System.Collections.Generic;

namespace DTOs
{
    public class DashboardDto
    {
        public class AdminOverviewResponse
        {
            public int TotalUsers { get; set; }
            public int TotalMangakas { get; set; }
            public int TotalSeries { get; set; }
            public int TotalChapters { get; set; }
        }

        public class AdminSeriesStatsResponse
        {
            public int PendingSeries { get; set; }
            public int ApprovedSeries { get; set; }
            public int RejectedSeries { get; set; }
            public int OngoingSeries { get; set; }
            public int CompletedSeries { get; set; }
        }
    }
}
