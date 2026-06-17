using DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardEvaluationController : ControllerBase
    {
        private readonly IBoardEvaluationService _service;

        public BoardEvaluationController(IBoardEvaluationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
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
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            return Ok(new
            {
                message = "EB evaluation deleted successfully"
            });
        }
    }
}