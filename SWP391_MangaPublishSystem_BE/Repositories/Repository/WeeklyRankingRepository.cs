using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class WeeklyRankingRepository : GenericRepository<WeeklyRanking>
    {
        public WeeklyRankingRepository() { }
        public WeeklyRankingRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task AddRangeAsync(IEnumerable<WeeklyRanking> rankings)
        {
            await _context.WeeklyRankings.AddRangeAsync(rankings);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WeeklyRanking>> GetLastNRankingsForSeriesAsync(int seriesId, int count)
        {
            return await _context.WeeklyRankings
                .Where(r => r.Seriesid == seriesId)
                .OrderByDescending(r => r.IssueYear)
                .ThenByDescending(r => r.IssueNumber)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<WeeklyRanking>> GetRankingsByIssueAsync(int issueYear, int issueNumber)
        {
            return await _context.WeeklyRankings
                .Include(r => r.Series)
                .Where(r => r.IssueYear == issueYear && r.IssueNumber == issueNumber)
                .OrderBy(r => r.Rankposition)
                .ToListAsync();
        }

        public async Task<List<WeeklyRanking>> GetRankingHistoryForSeriesAsync(int seriesId)
        {
            return await _context.WeeklyRankings
                .Include(r => r.Series)
                .Where(r => r.Seriesid == seriesId)
                .OrderByDescending(r => r.IssueYear)
                .ThenByDescending(r => r.IssueNumber)
                .ToListAsync();
        }
    }
}
