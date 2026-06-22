using DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangakaAssistantController : ControllerBase
    {
        private readonly IMangakaAssistantService _service;

        public MangakaAssistantController(IMangakaAssistantService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? mangakaId, [FromQuery] int? assistantId)
        {
            var result = await _service.GetAllAsync(mangakaId, assistantId);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Không tìm thấy hợp đồng.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MangakaAssistantDto.Create dto)
        {
            try
            {
                var id = await _service.CreateAsync(dto);
                return Ok(new
                {
                    message = "Mangaka assistant contract created successfully",
                    contractId = id
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MangakaAssistantDto.Update dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Mangaka assistant contract updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] MangakaAssistantDto.UpdateStatus dto)
        {
            try
            {
                await _service.UpdateStatusAsync(id, dto.Status);
                return Ok(new { message = "Mangaka assistant contract status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.RemoveAsync(id);
                return Ok(new { message = "Mangaka assistant contract soft deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}