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
            var id = await _service.CreateAsync(dto);

            if (id <= 0)
                return BadRequest("This assistant is already assigned to the mangaka!!!");

            return Ok(new
            {
                message = "Mangaka assistant contract created successfully",
                contractId = id
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MangakaAssistantDto.Update dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result <= 0)
                return NotFound("Contract not found !!!");

            return Ok(new
            {
                message = "Mangaka assistant contract updated successfully"
            });
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] MangakaAssistantDto.UpdateStatus dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);

            if (result <= 0)
                return NotFound("Contract not found!!!");

            return Ok(new
            {
                message = "Mangaka assistant contract status updated successfully"
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.RemoveAsync(id);

            if (!result)
                return NotFound("Contract not found!!!");

            return Ok(new
            {
                message = "Mangaka assistant contract soft deleted successfully"
            });
        }
    }
}