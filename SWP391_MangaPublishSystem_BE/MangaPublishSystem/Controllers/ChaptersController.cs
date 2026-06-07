using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChaptersController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChaptersController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? seriesId)
        {
            var result = await _chapterService.GetAllAsync(seriesId);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _chapterService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Không tìm thấy chương truyện có ID = {id}" });
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChapterDto.Create chapterDto)
        {
            if (chapterDto == null)
            {
                return BadRequest(new { Message = "Dữ liệu đầu vào không hợp lệ." });
            }

            var resultId = await _chapterService.CreateAsync(chapterDto);

            if (resultId <= 0)
            {
                return BadRequest(new { Message = "Tạo chương truyện thất bại." });
            }

            var fullData = await _chapterService.GetByIdAsync(resultId);

            return Ok(new
            {
                Message = "Created successfully",
                Id = resultId,
                Data = fullData
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ChapterDto.Update chapterDto)
        {
            chapterDto.Chapterid = id;

            var existing = await _chapterService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new { Message = "Không tìm thấy chương truyện cần cập nhật." });
            }

            var result = await _chapterService.UpdateAsync(chapterDto);
            if (result <= 0)
            {
                return BadRequest(new { Message = "Cập nhật dữ liệu thất bại." });
            }

            return NoContent(); 
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _chapterService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new { Message = "Không tìm thấy chương truyện để xóa." });
            }

            var success = await _chapterService.RemoveAsync(id);
            if (!success)
            {
                return BadRequest(new { Message = "Xóa thất bại." });
            }

            return Ok(new { Message = "Deleted successfully" });
        }
    }
}
