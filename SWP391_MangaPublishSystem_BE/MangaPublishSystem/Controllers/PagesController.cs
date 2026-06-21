using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;
        private readonly IPageLayerService _pageLayerService;
        private readonly IFileStorageService _fileStorage;

        public PagesController(IPageService pageService, IPageLayerService pageLayerService, IFileStorageService fileStorage)
        {
            _pageService = pageService;
            _pageLayerService = pageLayerService;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<PageDto>>> GetAll([FromQuery] int? chapterId)
        {
            var pages = await _pageService.GetAllAsync(chapterId);
            return Ok(pages);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PageDto>> GetById(int id)
        {
            var page = await _pageService.GetByIdAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            return Ok(page);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] PageDto.Create pageDto)
        {
            var result = await _pageService.CreateAsync(pageDto);

            if (result <= 0)
            {
                return BadRequest();
            }

            var layerDto = new PageLayerDto.Create
            {
                Pageid = result,
                Uploaderid = pageDto.Uploaderid,
                Layername = "Default",
                Zindex = 1
            };
            
            await _pageLayerService.CreateAsync(layerDto, uploadedUrl);

            return Ok(new
            {
                Message = "Created successfully",
                Id = result,
                Data = pageDto
            });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] PageDto.Update pageUpdateDto)
        {
            var existing = await _pageService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Không tìm thấy trang truyện cần cập nhật.");
            }

            var result = await _pageService.UpdateAsync(id, pageUpdateDto);
            if (result <= 0)
            {
                return BadRequest("Cập nhật thất bại.");
            }

            return NoContent();
        }

        [HttpPost("{id:int}/composite")]
        public async Task<ActionResult> CompositeImage(int id)
        {
            var result = await _pageService.CompositeAndSaveImageAsync(id);
            if (result == null)
            {
                return BadRequest(new { Message = "Không thể ghép ảnh hoặc không có layer hợp lệ." });
            }

            return Ok(new { Message = "Composite successfully", Pageimageurl = result });
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _pageService.UpdateStatusAsync(id, status);
            if (!success)
            {
                return NotFound("Không tìm thấy trang truyện để cập nhật trạng thái.");
            }
            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpDelete("{id:int}/soft")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var success = await _pageService.SoftDeleteAsync(id);
            if (!success)
            {
                return NotFound("Không tìm thấy trang truyện để xóa tạm.");
            }
            return Ok(new { Message = "Soft deleted successfully" });
        }

        //[HttpDelete("{id:int}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var result = await _pageService.RemoveAsync(id);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}
    }
}
