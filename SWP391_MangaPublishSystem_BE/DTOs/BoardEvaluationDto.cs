using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class BoardEvaluationDto
    {
        public class Create
        {
            public int Seriesid { get; set; }
            public int Inputtedbyid { get; set; }

            public decimal StoryScore { get; set; }
            public decimal ArtScore { get; set; }
            public decimal CharacterScore { get; set; }
            public decimal CommercialScore { get; set; }
            public decimal PacingScore { get; set; }

            public string? Feedback { get; set; }
        }

        public class Update
        {
            public decimal StoryScore { get; set; }
            public decimal ArtScore { get; set; }
            public decimal CharacterScore { get; set; }
            public decimal CommercialScore { get; set; }
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
