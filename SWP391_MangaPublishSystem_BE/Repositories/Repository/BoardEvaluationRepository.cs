using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;



namespace Repositories.Repository
{
    public class BoardEvaluationRepository : GenericRepository<BoardEvaluation>
    {
        /*
         * public BoardEvaluationRepository(MangaPublishDbContext context)
            : base(context)
        {
        }
        */
        public async Task<List<BoardEvaluation>> GetBySeriesIdAsync(int seriesId)
        {
            return await _context.BoardEvaluations
                .Where(x => x.Seriesid == seriesId)
                .ToListAsync();
        }
        public async Task<List<BoardEvaluation>> GetAllAsync()
        {
            return await _context.BoardEvaluations
                .Include(x => x.Series)
                .Include(x => x.Inputtedby)
                .ToListAsync();
        }

        public async Task<BoardEvaluation?> GetByIdAsync(int id)
        {
            return await _context.BoardEvaluations
                .Include(x => x.Series)
                .Include(x => x.Inputtedby)
                .FirstOrDefaultAsync(x => x.Evaluationid == id);
        }

        public async Task<bool> DeleteAsync(BoardEvaluation evaluation)
        {
            _context.BoardEvaluations.Remove(evaluation);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<int> CreateBatchAsync(
    BoardEvaluation evaluation,
    List<BoardEvaluationDetail> details)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.BoardEvaluations.AddAsync(evaluation);
            await _context.SaveChangesAsync();

            foreach (var detail in details)
            {
                detail.EvaluationId = evaluation.Evaluationid;
            }

            await _context.BoardEvaluationDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return evaluation.Evaluationid;
        }

        public async Task<BoardEvaluation?> GetBatchSummaryAsync(int evaluationId)
        {
            return await _context.BoardEvaluations
                .Include(x => x.BoardEvaluationDetails)
                    .ThenInclude(d => d.Eb)
                .FirstOrDefaultAsync(x => x.Evaluationid == evaluationId);
        }

        public async Task<BoardEvaluation?> GetBySeriesIdWithDetailsAsync(int seriesId)
        {
            return await _context.BoardEvaluations
                .Include(x => x.BoardEvaluationDetails)
                    .ThenInclude(d => d.Eb)
                .FirstOrDefaultAsync(x => x.Seriesid == seriesId);
        }

        public async Task SaveDetailAsync(BoardEvaluationDetail detail)
        {
            var existing = await _context.BoardEvaluationDetails
                .FirstOrDefaultAsync(x => x.EvaluationId == detail.EvaluationId && x.EbId == detail.EbId);

            if (existing == null)
            {
                await _context.BoardEvaluationDetails.AddAsync(detail);
            }
            else
            {
                existing.StoryScore = detail.StoryScore;
                existing.ArtScore = detail.ArtScore;
                existing.CharacterScore = detail.CharacterScore;
                existing.CommercialScore = detail.CommercialScore;
                existing.PacingScore = detail.PacingScore;
                existing.Feedback = detail.Feedback;
                existing.EvaluatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveMainEvaluationAsync(BoardEvaluation evaluation)
        {
            var existing = await _context.BoardEvaluations
                .FirstOrDefaultAsync(x => x.Evaluationid == evaluation.Evaluationid);

            if (existing != null)
            {
                existing.StoryScore = evaluation.StoryScore;
                existing.ArtScore = evaluation.ArtScore;
                existing.CharacterScore = evaluation.CharacterScore;
                existing.CommercialScore = evaluation.CommercialScore;
                existing.PacingScore = evaluation.PacingScore;
                existing.AverageScore = evaluation.AverageScore;
                existing.FinalDecision = evaluation.FinalDecision;
                existing.ApprovedPublishFormat = evaluation.ApprovedPublishFormat;
                existing.Feedback = evaluation.Feedback;
                existing.Evaluatedat = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}