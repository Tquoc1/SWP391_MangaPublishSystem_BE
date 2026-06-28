using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISeriesService _seriesService;
        private readonly IBoardEvaluationService _evaluationService;

        public SubmissionsController(ISeriesService seriesService, IBoardEvaluationService evaluationService)
        {
            _seriesService = seriesService;
            _evaluationService = evaluationService;
        }

        [HttpGet("eb")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> GetEditorialBoardQueue()
        {
            // Fetch series with status "Submitted" or "UnderReview"
            var submittedSeries = await _seriesService.GetAllAsync(null, "Submitted");
            var underReviewSeries = await _seriesService.GetAllAsync(null, "UnderReview");

            var combined = submittedSeries.Concat(underReviewSeries)
                .OrderByDescending(s => s.Createdat)
                .ToList();

            return Ok(combined);
        }

        [HttpGet("{seriesId:int}/evaluations")]
        [Authorize(Roles = "Admin, EB, Editor, Mangaka")]
        public async Task<IActionResult> GetEvaluationsBySeries(int seriesId)
        {
            var result = await _evaluationService.GetBySeriesIdSummaryAsync(seriesId);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá cho series này." });
            }
            return Ok(result);
        }

        [HttpPatch("{seriesId:int}/score")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> SavePartialScore(int seriesId, [FromBody] BoardEvaluationDto.PartialGradeInput dto)
        {
            try
            {
                await _evaluationService.SavePartialMemberScoreAsync(seriesId, dto);
                return Ok(new { message = "Lưu điểm thành viên thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{seriesId:int}/evaluators-status")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> GetEvaluatorsStatus(int seriesId)
        {
            try
            {
                var result = await _evaluationService.GetEvaluatorsStatusAsync(seriesId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
