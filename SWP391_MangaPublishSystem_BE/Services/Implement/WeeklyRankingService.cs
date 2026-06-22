using ClosedXML.Excel;
using DTOs;
using Entities.Models;
using Repositories.Repository;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class WeeklyRankingService : IWeeklyRankingService
    {
        private readonly WeeklyRankingRepository _weeklyRankingRepository;
        private readonly SeriesRepository _seriesRepository;

        // Tiêu chí: bao nhiêu truyện có rank thấp nhất bị tính là Bottom Rank
        private const int BottomRankThresholdCount = 3;
        // Tiêu chí: bao nhiêu tuần liên tiếp Bottom Rank thì bị dừng xuất bản
        private const int ConsecutiveBottomRankLimit = 3;

        public WeeklyRankingService(WeeklyRankingRepository weeklyRankingRepository, SeriesRepository seriesRepository)
        {
            _weeklyRankingRepository = weeklyRankingRepository;
            _seriesRepository = seriesRepository;
        }

        public async Task<bool> ImportWeeklyRankingAsync(int inputtedById, WeeklyRankingDto.Import dto)
        {
            var excelData = new List<(int SeriesId, int VoteCount)>();

            using (var stream = new MemoryStream())
            {
                await dto.ExcelFile.CopyToAsync(stream);
                stream.Position = 0;

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) return false;

                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

                    foreach (var row in rows)
                    {
                        var seriesIdStr = row.Cell(1).Value.ToString();
                        var voteCountStr = row.Cell(2).Value.ToString();

                        if (int.TryParse(seriesIdStr, out var seriesId) && int.TryParse(voteCountStr, out var voteCount))
                        {
                            excelData.Add((seriesId, voteCount));
                        }
                    }
                }
            }

            if (!excelData.Any()) return false;

            // Tính Rank
            var rankedData = excelData.OrderByDescending(x => x.VoteCount).ToList();
            var totalCount = rankedData.Count;

            var rankingsToInsert = new List<WeeklyRanking>();
            var bottomRankedSeriesIds = new List<int>();

            for (int i = 0; i < totalCount; i++)
            {
                int rankPosition = i + 1;
                // Nếu thuộc nhóm BottomRankThresholdCount thấp nhất => IsBottomRank = true
                bool isBottomRank = rankPosition > (totalCount - BottomRankThresholdCount);

                var item = rankedData[i];
                var ranking = new WeeklyRanking
                {
                    Seriesid = item.SeriesId,
                    IssueNumber = dto.IssueNumber,
                    IssueYear = dto.IssueYear,
                    Votecount = item.VoteCount,
                    Inputtedbyid = inputtedById,
                    Recordedat = DateTime.UtcNow,
                    Rankposition = rankPosition,
                    Isbottomrank = isBottomRank,
                    Calculatedat = DateTime.UtcNow
                };

                rankingsToInsert.Add(ranking);

                if (isBottomRank)
                {
                    bottomRankedSeriesIds.Add(item.SeriesId);
                }
            }

            await _weeklyRankingRepository.AddRangeAsync(rankingsToInsert);

            // Kiểm tra các truyện bị bottom rank, xem đã đủ ConsecutiveBottomRankLimit tuần liên tiếp chưa
            foreach (var seriesId in bottomRankedSeriesIds)
            {
                var history = await _weeklyRankingRepository.GetLastNRankingsForSeriesAsync(seriesId, ConsecutiveBottomRankLimit);
                
                if (history.Count == ConsecutiveBottomRankLimit && history.All(h => h.Isbottomrank == true))
                {
                    // Cập nhật trạng thái Dừng xuất bản (Cancelled)
                    var series = await _seriesRepository.GetByIdAsync(seriesId);
                    if (series != null && series.Status != "Cancelled")
                    {
                        series.Status = "Cancelled";
                        await _seriesRepository.UpdateAsync(series);
                    }
                }
            }

            return true;
        }

        public async Task<List<WeeklyRankingDto.Response>> GetRankingsByIssueAsync(int issueYear, int issueNumber)
        {
            var rankings = await _weeklyRankingRepository.GetRankingsByIssueAsync(issueYear, issueNumber);
            return rankings.Select(r => new WeeklyRankingDto.Response
            {
                RankingId = r.Rankingid,
                SeriesId = r.Seriesid,
                SeriesTitle = r.Series?.Title,
                IssueNumber = r.IssueNumber,
                IssueYear = r.IssueYear,
                VoteCount = r.Votecount,
                RankPosition = r.Rankposition,
                IsBottomRank = r.Isbottomrank,
                RecordedAt = r.Recordedat
            }).ToList();
        }

        public async Task<List<WeeklyRankingDto.Response>> GetRankingHistoryForSeriesAsync(int seriesId)
        {
            var rankings = await _weeklyRankingRepository.GetRankingHistoryForSeriesAsync(seriesId);
            return rankings.Select(r => new WeeklyRankingDto.Response
            {
                RankingId = r.Rankingid,
                SeriesId = r.Seriesid,
                SeriesTitle = r.Series?.Title,
                IssueNumber = r.IssueNumber,
                IssueYear = r.IssueYear,
                VoteCount = r.Votecount,
                RankPosition = r.Rankposition,
                IsBottomRank = r.Isbottomrank,
                RecordedAt = r.Recordedat
            }).ToList();
        }
    }
}
