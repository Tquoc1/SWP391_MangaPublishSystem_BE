using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IWeeklyRankingService
    {
        Task<bool> ImportWeeklyRankingAsync(int inputtedById, WeeklyRankingDto.Import dto);
        Task<List<WeeklyRankingDto.Response>> GetRankingsByIssueAsync(int issueYear, int issueNumber);
        Task<List<WeeklyRankingDto.Response>> GetRankingHistoryForSeriesAsync(int seriesId);
    }
}
