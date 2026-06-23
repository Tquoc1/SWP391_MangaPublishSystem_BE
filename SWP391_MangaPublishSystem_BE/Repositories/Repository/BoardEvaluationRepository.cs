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
                .FirstOrDefaultAsync(x => x.Evaluationid == evaluationId);
        }
    }
}