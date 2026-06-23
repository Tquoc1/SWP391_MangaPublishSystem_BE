using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class WeeklyRankingDto
    {
        public class Import
        {
            [Required(ErrorMessage = "Vui lòng đính kèm file Excel.")]
            public IFormFile ExcelFile { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập Issue Number.")]
            [Range(1, 53, ErrorMessage = "Issue Number phải từ 1 đến 53.")]
            public int IssueNumber { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập Issue Year.")]
            public int IssueYear { get; set; }
        }

        public class Response
        {
            public int RankingId { get; set; }
            public int SeriesId { get; set; }
            public string SeriesTitle { get; set; }
            public int IssueNumber { get; set; }
            public int IssueYear { get; set; }
            public int? VoteCount { get; set; }
            public int? RankPosition { get; set; }
            public bool? IsBottomRank { get; set; }
            public System.DateTime? RecordedAt { get; set; }
        }
    }
}
