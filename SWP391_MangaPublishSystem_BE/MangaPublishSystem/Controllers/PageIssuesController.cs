using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Editor, EB")]
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

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Mangaka, Assistant, Editor, EB")]
        public async Task<IActionResult> UpdateStatus(int id, PageIssueDto.UpdateStatus dto)
        {
            try
            {
                await _pageIssueService.UpdateStatusAsync(id, dto);
                return Ok(new { message = "PageIssue status updated successfully" });
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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Mangaka, Assistant, Editor, EB")]
        public async Task<ActionResult> Update(int id, [FromBody] PageIssueDto.Update pageDto)
        {
            try
            {
                await _pageIssueService.UpdateAsync(id, pageDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}/soft")]
        [Authorize(Roles = "Editor, EB, Admin")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            try
            {
                await _pageIssueService.SoftDeleteAsync(id);
                return Ok(new { Message = "Soft deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        //[HttpDelete("{id:int}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var result = await _pageIssueService.RemoveAsync(id);
        //    if (!result)
        //    {
        //        return NotFound("Sự cố không tồn tại hoặc đã bị xóa trước đó.");
        //    }

        //    return NoContent();
        //}
    }
}
