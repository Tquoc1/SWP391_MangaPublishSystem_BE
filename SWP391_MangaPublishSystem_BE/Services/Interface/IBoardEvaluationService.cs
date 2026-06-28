using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Services.DTO;

namespace Services.Interface
{
    public interface IBoardEvaluationService
    {
        Task<List<BoardEvaluationDto.Response>> GetAllAsync();
        Task<BoardEvaluationDto.Response> GetByIdAsync(int id);
        Task<int> CreateAsync(BoardEvaluationDto.Create dto);
        Task UpdateAsync(int id, BoardEvaluationDto.Update dto);
        Task DeleteAsync(int id);
        Task<int> CreateBatchAsync(BoardEvaluationDto.CreateBatch dto);
        Task<BoardEvaluationDto.BatchSummary?> GetBatchSummaryAsync(int evaluationId);
        Task<BoardEvaluationDto.BatchSummary?> GetBySeriesIdSummaryAsync(int seriesId);
        Task SavePartialMemberScoreAsync(int seriesId, BoardEvaluationDto.PartialGradeInput dto);
        Task<List<BoardEvaluationDto.EvaluatorStatusResponse>> GetEvaluatorsStatusAsync(int seriesId);
    }
}
