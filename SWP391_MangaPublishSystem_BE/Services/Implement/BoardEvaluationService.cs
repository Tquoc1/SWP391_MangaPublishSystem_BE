using DTOs;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Repository;
using Services.DTO;
using Services.Interface;

namespace Services.Implement
{
    public class BoardEvaluationService : IBoardEvaluationService
    {
        private readonly BoardEvaluationRepository _repository;
        private readonly SeriesRepository _seriesRepository;
        private readonly INotificationService _notificationService;

        public BoardEvaluationService(
            BoardEvaluationRepository repository,
            SeriesRepository seriesRepository,
            INotificationService notificationService)
        {
            _repository = repository;
            _seriesRepository = seriesRepository;
            _notificationService = notificationService;
        }

        public async Task<List<BoardEvaluationDto.Response>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();

            return data.Select(e => new BoardEvaluationDto.Response
            {
                Evaluationid = e.Evaluationid,
                Seriesid = e.Seriesid,
                Inputtedbyid = e.Inputtedbyid,
                StoryScore = e.StoryScore,
                ArtScore = e.ArtScore,
                CharacterScore = e.CharacterScore,
                CommercialScore = e.CommercialScore,
                PacingScore = e.PacingScore,
                AverageScore = e.AverageScore ?? 0,
                FinalDecision = e.FinalDecision,
                ApprovedPublishFormat = e.ApprovedPublishFormat,
                Feedback = e.Feedback,
                Evaluatedat = e.Evaluatedat
            }).ToList();
        }

        public async Task<BoardEvaluationDto.Response?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new BoardEvaluationDto.Response
            {
                Evaluationid = e.Evaluationid,
                Seriesid = e.Seriesid,
                Inputtedbyid = e.Inputtedbyid,
                StoryScore = e.StoryScore,
                ArtScore = e.ArtScore,
                CharacterScore = e.CharacterScore,
                CommercialScore = e.CommercialScore,
                PacingScore = e.PacingScore,
                AverageScore = e.AverageScore ?? 0,
                FinalDecision = e.FinalDecision,
                ApprovedPublishFormat = e.ApprovedPublishFormat,
                Feedback = e.Feedback,
                Evaluatedat = e.Evaluatedat
            };
        }

        /*public async Task<int> CreateAsync(BoardEvaluationDto.Create dto)
        {
            decimal average =
                (dto.StoryScore +
                 dto.ArtScore +
                 dto.CharacterScore +
                 dto.CommercialScore +
                 dto.PacingScore) / 5m;

            bool isApproved = average >= 5m;

            var evaluation = new BoardEvaluation
            {
                Seriesid = dto.Seriesid,
                Inputtedbyid = dto.Inputtedbyid,
                StoryScore = dto.StoryScore,
                ArtScore = dto.ArtScore,
                CharacterScore = dto.CharacterScore,
                CommercialScore = dto.CommercialScore,
                PacingScore = dto.PacingScore,
                FinalDecision = isApproved ? "Approve" : "Reject",
                ApprovedPublishFormat = isApproved ? "Weekly" : null,
                Feedback = dto.Feedback,
                Evaluatedat = DateTime.Now
            };

            await _repository.CreateAsync(evaluation);

            var series = await _seriesRepository.GetByIdAsync(dto.Seriesid);
            //if (series != null)
            //{
            //    series.Status = isApproved ? "Approved" : "Reject";
            //    series.Publishformat = isApproved ? "Weekly" : null;
            //    series.Approvedat = isApproved ? DateTime.Now : null;

            //    await _seriesRepository.UpdateAsync(series);
            //}

            return evaluation.Evaluationid;
        }*/
        public async Task<int> CreateAsync(BoardEvaluationDto.Create dto)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(dto.Seriesid);
            if (series == null || series.Isdeleted == true)
            {
                throw new KeyNotFoundException("Series không tồn tại.");
            }
            decimal average =
                (dto.StoryScore +
                 dto.ArtScore +
                 dto.CharacterScore +
                 dto.CommercialScore +
                 dto.PacingScore) / 5m;

            var evaluation = new BoardEvaluation
            {
                Seriesid = dto.Seriesid,
                Inputtedbyid = dto.Inputtedbyid,
                StoryScore = dto.StoryScore,
                ArtScore = dto.ArtScore,
                CharacterScore = dto.CharacterScore,
                CommercialScore = dto.CommercialScore,
                PacingScore = dto.PacingScore,
                AverageScore = average,
                Feedback = dto.Feedback,
                Evaluatedat = DateTime.UtcNow
            };

            await _repository.CreateAsync(evaluation);

            await RecalculateSeriesEvaluationAsync(dto.Seriesid);
            
            if (series.Mangakaid > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    series.Mangakaid,
                    "Có đánh giá mới từ Hội đồng",
                    $"Tác phẩm '{series.Title}' của bạn vừa nhận được một đánh giá mới.",
                    series.Seriesid
                );
            }

            return evaluation.Evaluationid;
        }

        /*
         public async Task<bool> UpdateAsync(int id, BoardEvaluationDto.Update dto)
        {
            var evaluation = await _repository.GetByIdAsync(id);
            if (evaluation == null) return false;

            decimal average =
                (dto.StoryScore +
                 dto.ArtScore +
                 dto.CharacterScore +
                 dto.CommercialScore +
                 dto.PacingScore) / 5m;

            bool isApproved = average >= 5m;

            evaluation.StoryScore = dto.StoryScore;
            evaluation.ArtScore = dto.ArtScore;
            evaluation.CharacterScore = dto.CharacterScore;
            evaluation.CommercialScore = dto.CommercialScore;
            evaluation.PacingScore = dto.PacingScore;
            evaluation.FinalDecision = isApproved ? "Approve" : "Reject";
            evaluation.ApprovedPublishFormat = isApproved ? "Weekly" : null;
            evaluation.Feedback = dto.Feedback;
            evaluation.Evaluatedat = DateTime.Now;

            var rows = await _repository.UpdateAsync(evaluation);

            var series = await _seriesRepository.GetByIdAsync(evaluation.Seriesid);
            //if (series != null)
            //{
            //    series.Status = isApproved ? "Approved" : "Reject";
            //    series.Publishformat = isApproved ? "Weekly" : null;
            //    series.Approvedat = isApproved ? DateTime.Now : null;

            //    await _seriesRepository.UpdateAsync(series);
            //}

            return rows > 0;
        }
        */
        public async Task UpdateAsync(int id, BoardEvaluationDto.Update dto)
        {
            var evaluation = await _repository.GetByIdAsync(id);
            if (evaluation == null) throw new KeyNotFoundException("Không tìm thấy đánh giá hội đồng.");

            decimal average =
                (dto.StoryScore +
                 dto.ArtScore +
                 dto.CharacterScore +
                 dto.CommercialScore +
                 dto.PacingScore) / 5m;

            evaluation.StoryScore = dto.StoryScore;
            evaluation.ArtScore = dto.ArtScore;
            evaluation.CharacterScore = dto.CharacterScore;
            evaluation.CommercialScore = dto.CommercialScore;
            evaluation.PacingScore = dto.PacingScore;
            evaluation.AverageScore = average;
            evaluation.Feedback = dto.Feedback;
            evaluation.Evaluatedat = DateTime.UtcNow;

            await _repository.UpdateAsync(evaluation);

            await RecalculateSeriesEvaluationAsync(evaluation.Seriesid);
        }
        public async Task DeleteAsync(int id)
        {
            var evaluation = await _repository.GetByIdAsync(id);
            if (evaluation == null) throw new KeyNotFoundException("Không tìm thấy đánh giá hội đồng để xóa.");

            await _repository.DeleteAsync(evaluation);
        }

        public async Task<int> CreateBatchAsync(BoardEvaluationDto.CreateBatch dto)
        {
            if (dto.Evaluators == null || !dto.Evaluators.Any())
                throw new InvalidOperationException("Evaluator list cannot be empty.");

            var series = await _seriesRepository.GetByIdWithDetailsAsync(dto.Seriesid);
            if (series == null || series.Isdeleted == true)
                throw new KeyNotFoundException("Series does not exist.");

            var duplicateEb = dto.Evaluators
                .GroupBy(x => x.EbId)
                .Any(g => g.Count() > 1);

            if (duplicateEb)
                throw new InvalidOperationException("One EB cannot appear more than once in the same evaluation.");

            decimal avgStory = dto.Evaluators.Average(x => x.StoryScore);
            decimal avgArt = dto.Evaluators.Average(x => x.ArtScore);
            decimal avgCharacter = dto.Evaluators.Average(x => x.CharacterScore);
            decimal avgCommercial = dto.Evaluators.Average(x => x.CommercialScore);
            decimal avgPacing = dto.Evaluators.Average(x => x.PacingScore);

            decimal finalAverage =
                (avgStory + avgArt + avgCharacter + avgCommercial + avgPacing) / 5m;

            bool isApproved = finalAverage >= 5m;

            var evaluation = new BoardEvaluation
            {
                Seriesid = dto.Seriesid,
                Inputtedbyid = dto.Inputtedbyid,

                StoryScore = avgStory,
                ArtScore = avgArt,
                CharacterScore = avgCharacter,
                CommercialScore = avgCommercial,
                PacingScore = avgPacing,

                AverageScore = finalAverage,
                FinalDecision = isApproved ? "Approve" : "Reject",
                ApprovedPublishFormat = isApproved ? "Weekly" : null,
                Feedback = dto.Feedback,
                Evaluatedat = DateTime.UtcNow
            };

            var details = dto.Evaluators.Select(x => new BoardEvaluationDetail
            {
                EbId = x.EbId,
                StoryScore = x.StoryScore,
                ArtScore = x.ArtScore,
                CharacterScore = x.CharacterScore,
                CommercialScore = x.CommercialScore,
                PacingScore = x.PacingScore,
                Feedback = x.Feedback,
                EvaluatedAt = DateTime.UtcNow
            }).ToList();

            var evaluationId = await _repository.CreateBatchAsync(evaluation, details);

            series.Status = isApproved ? "Approved" : "Rejected";
            series.Publishformat = isApproved ? "Weekly" : "Pending";
            series.Approvedat = isApproved ? DateTime.UtcNow : null;

            await _seriesRepository.UpdateAsync(series);

            return evaluationId;
        }

        public async Task<BoardEvaluationDto.BatchSummary?> GetBatchSummaryAsync(int evaluationId)
        {
            var evaluation = await _repository.GetBatchSummaryAsync(evaluationId);

            if (evaluation == null)
                return null;

            var details = evaluation.BoardEvaluationDetails.ToList();

            return new BoardEvaluationDto.BatchSummary
            {
                Evaluationid = evaluation.Evaluationid,
                Seriesid = evaluation.Seriesid,

                Evaluations = details.Select(x => new BoardEvaluationDto.DetailResponse
                {
                    DetailId = x.DetailId,
                    EbId = x.EbId,
                    StoryScore = x.StoryScore,
                    ArtScore = x.ArtScore,
                    CharacterScore = x.CharacterScore,
                    CommercialScore = x.CommercialScore,
                    PacingScore = x.PacingScore,
                    AverageScore =
                        (x.StoryScore +
                         x.ArtScore +
                         x.CharacterScore +
                         x.CommercialScore +
                         x.PacingScore) / 5m,
                    Feedback = x.Feedback
                }).ToList(),

                AverageStoryScore = details.Average(x => x.StoryScore),
                AverageArtScore = details.Average(x => x.ArtScore),
                AverageCharacterScore = details.Average(x => x.CharacterScore),
                AverageCommercialScore = details.Average(x => x.CommercialScore),
                AveragePacingScore = details.Average(x => x.PacingScore),

                FinalAverageScore = evaluation.AverageScore ?? 0,
                Decision = evaluation.FinalDecision
            };
        }
        private async Task RecalculateSeriesEvaluationAsync(int seriesId)
        {
            var evaluations = await _repository.GetBySeriesIdAsync(seriesId);

            if (evaluations == null || !evaluations.Any())
                return;

            decimal avgStory = evaluations.Average(x => x.StoryScore);
            decimal avgArt = evaluations.Average(x => x.ArtScore);
            decimal avgCharacter = evaluations.Average(x => x.CharacterScore);
            decimal avgCommercial = evaluations.Average(x => x.CommercialScore);
            decimal avgPacing = evaluations.Average(x => x.PacingScore);

            decimal finalAverage =
                (avgStory + avgArt + avgCharacter + avgCommercial + avgPacing) / 5m;

            bool isApproved = finalAverage >= 5m;

            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId);
            if (series == null || series.Isdeleted == true)
                return;

            series.Status = isApproved ? "Approved" : "Rejected";
            series.Publishformat = isApproved ? "Weekly" : "Pending";
            series.Approvedat = isApproved ? DateTime.UtcNow : null;

            await _seriesRepository.UpdateAsync(series);
        }
    }
}