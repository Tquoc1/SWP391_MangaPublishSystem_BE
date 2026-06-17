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

        public BoardEvaluationService(
            BoardEvaluationRepository repository,
            SeriesRepository seriesRepository)
        {
            _repository = repository;
            _seriesRepository = seriesRepository;
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
                throw new Exception("Series không tồn tại.");
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

            evaluation.StoryScore = dto.StoryScore;
            evaluation.ArtScore = dto.ArtScore;
            evaluation.CharacterScore = dto.CharacterScore;
            evaluation.CommercialScore = dto.CommercialScore;
            evaluation.PacingScore = dto.PacingScore;
            evaluation.AverageScore = average;
            evaluation.Feedback = dto.Feedback;
            evaluation.Evaluatedat = DateTime.UtcNow;

            var rows = await _repository.UpdateAsync(evaluation);

            await RecalculateSeriesEvaluationAsync(evaluation.Seriesid);

            return rows > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var evaluation = await _repository.GetByIdAsync(id);
            if (evaluation == null) return false;

            return await _repository.DeleteAsync(evaluation);
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