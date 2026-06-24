using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class BoardEvaluationDto
    {
        public class Create
        {
            [Required(ErrorMessage = "Series ID không được để trống")]
            public int Seriesid { get; set; }

            [Required(ErrorMessage = "Người nhập không được để trống")]
            public int Inputtedbyid { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal StoryScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal ArtScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CharacterScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CommercialScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal PacingScore { get; set; }

            public string? Feedback { get; set; }
        }
        public class CreateBatch
        {
            [Required(ErrorMessage = "Series ID không được để trống")]
            public int Seriesid { get; set; }

            [Required(ErrorMessage = "Người nhập không được để trống")]
            public int Inputtedbyid { get; set; }

            public string? Feedback { get; set; }

            [MinLength(1, ErrorMessage = "Cần ít nhất một đánh giá từ ban giám khảo")]
            public List<EbScore> Evaluators { get; set; } = new();
        }
        public class EbScore
        {
            [Required(ErrorMessage = "Mã giám khảo không được để trống")]
            public int EbId { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal StoryScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal ArtScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CharacterScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CommercialScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal PacingScore { get; set; }

            public string? Feedback { get; set; }
        }
        public class DetailResponse
        {
            public int DetailId { get; set; }
            public int EbId { get; set; }
            public string? Fullname { get; set; }
            public string? Username { get; set; }

            public decimal StoryScore { get; set; }
            public decimal ArtScore { get; set; }
            public decimal CharacterScore { get; set; }
            public decimal CommercialScore { get; set; }
            public decimal PacingScore { get; set; }

            public decimal AverageScore { get; set; }
            public string? Feedback { get; set; }
        }

        public class PartialGradeInput
        {
            [Required(ErrorMessage = "Mã giám khảo không được để trống")]
            public int EbId { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal StoryScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal ArtScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CharacterScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CommercialScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal PacingScore { get; set; }

            public string? Feedback { get; set; }
        }

        public class BatchSummary
        {
            public int Evaluationid { get; set; }
            public int Seriesid { get; set; }

            public List<DetailResponse> Evaluations { get; set; } = new();

            public decimal AverageStoryScore { get; set; }
            public decimal AverageArtScore { get; set; }
            public decimal AverageCharacterScore { get; set; }
            public decimal AverageCommercialScore { get; set; }
            public decimal AveragePacingScore { get; set; }

            public decimal FinalAverageScore { get; set; }
            public string Decision { get; set; } = string.Empty;
        }
        public class Update
        {
            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal StoryScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal ArtScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CharacterScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal CommercialScore { get; set; }

            [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
            public decimal PacingScore { get; set; }

            public string? Feedback { get; set; }
        }

        public class Response
        {
            public int Evaluationid { get; set; }

            public int Seriesid { get; set; }
            public int Inputtedbyid { get; set; }

            public decimal StoryScore { get; set; }
            public decimal ArtScore { get; set; }
            public decimal CharacterScore { get; set; }
            public decimal CommercialScore { get; set; }
            public decimal PacingScore { get; set; }

            public decimal AverageScore { get; set; }

            public string? FinalDecision { get; set; }
            public string? ApprovedPublishFormat { get; set; }
            public string? Feedback { get; set; }

            public DateTime? Evaluatedat { get; set; }
        }
    }
}
