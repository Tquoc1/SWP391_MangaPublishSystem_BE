using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardEvaluationController : ControllerBase
    {
        private readonly IBoardEvaluationService _service;

        public BoardEvaluationController(IBoardEvaluationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, EB, Editor, Mangaka")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, EB, Editor, Mangaka")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> Create([FromBody] BoardEvaluationDto.Create dto)
        {
            var id = await _service.CreateAsync(dto);

            return Ok(new
            {
                message = "EB evaluated series successfully",
                evaluationId = id
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> Update(int id, [FromBody] BoardEvaluationDto.Update dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();

            return Ok(new
            {
                message = "EB evaluation updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            return Ok(new
            {
                message = "EB evaluation deleted successfully"
            });
        }
        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatch([FromBody] BoardEvaluationDto.CreateBatch dto)
        {
            try
            {
                var id = await _service.CreateBatchAsync(dto);

                return Ok(new
                {
                    message = "Board evaluation batch created successfully.",
                    evaluationId = id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("{evaluationId:int}/summary")]
        public async Task<IActionResult> GetBatchSummary(int evaluationId)
        {
            var result = await _service.GetBatchSummaryAsync(evaluationId);

            if (result == null)
                return NotFound("Evaluation not found.");

            return Ok(result);
        }
    }
}