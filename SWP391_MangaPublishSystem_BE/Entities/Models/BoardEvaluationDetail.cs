namespace Entities.Models;

public partial class BoardEvaluationDetail
{
    public int DetailId { get; set; }

    public int EvaluationId { get; set; }

    public int EbId { get; set; }

    public decimal StoryScore { get; set; }

    public decimal ArtScore { get; set; }

    public decimal CharacterScore { get; set; }

    public decimal CommercialScore { get; set; }

    public decimal PacingScore { get; set; }

    public decimal? AverageScore { get; set; }

    public string? Feedback { get; set; }

    public DateTime? EvaluatedAt { get; set; }
    public bool? Isdeleted { get; set; }

    public virtual BoardEvaluation Evaluation { get; set; }

    public virtual User Eb { get; set; }
}