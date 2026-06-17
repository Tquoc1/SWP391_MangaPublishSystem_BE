using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageIssuesController : ControllerBase
    {
        private readonly IPageIssueService _pageIssueService;

        public PageIssuesController(IPageIssueService pageIssueService)
        {
            _pageIssueService = pageIssueService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PageIssueDto>>> GetAll([FromQuery] int? chapterId)
        {
            var issues = await _pageIssueService.GetAllAsync(chapterId);
            return Ok(issues);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PageIssueDto>> GetById(int id)
        {
            var issue = await _pageIssueService.GetByIdAsync(id);
            if (issue == null)
            {
                return NotFound("Không tìm thấy sự cố (Issue) yêu cầu hoặc sự cố đã bị xóa.");
            }
            return Ok(issue);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] PageIssueDto.Create pageDto)
        {
            if (pageDto == null)
            {
                return BadRequest("Dữ liệu gửi lên không hợp lệ.");
            }

            var resultId = await _pageIssueService.CreateAsync(pageDto);
            if (resultId <= 0)
            {
                return BadRequest("Tạo sự cố lỗi trang thất bại.");
            }

            return Ok(new
            {
                Message = "Created successfully",
                Id = resultId,
                Data = pageDto
            });
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] PageIssueDto.Update pageDto)
        {
            if (pageDto == null)
            {
                return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            }

            var result = await _pageIssueService.UpdateAsync(id, pageDto);
            if (result <= 0)
            {
                return BadRequest("Cập nhật thông tin sự cố thất bại.");
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _pageIssueService.UpdateStatusAsync(id, status);
            if (!success)
            {
                return NotFound("Không tìm thấy sự cố để cập nhật trạng thái.");
            }
            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpDelete("{id:int}/soft")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var success = await _pageIssueService.SoftDeleteAsync(id);
            if (!success)
            {
                return NotFound("Không tìm thấy sự cố để xóa tạm.");
            }
            return Ok(new { Message = "Soft deleted successfully" });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _pageIssueService.RemoveAsync(id);
            if (!result)
            {
                return NotFound("Sự cố không tồn tại hoặc đã bị xóa trước đó.");
            }

            return NoContent();
        }
    }
}
