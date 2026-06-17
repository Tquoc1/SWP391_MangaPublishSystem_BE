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
        private readonly IFileStorageService _fileStorage;

        public PagesController(IPageService pageService, IFileStorageService fileStorage)
        {
            _pageService = pageService;
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Create([FromForm] PageDto.Create pageDto, IFormFile pageFile)
        {
            if (pageFile == null || pageFile.Length == 0)
            {
                return BadRequest("Vui lòng tải lên hình ảnh cho trang truyện.");
            }

            await using var stream = pageFile.OpenReadStream();
            string uploadedUrl = await _fileStorage.UploadAsync(
                stream, pageFile.FileName, pageFile.ContentType, "manga-pages");

            var result = await _pageService.CreateAsync(pageDto, uploadedUrl);

            if (result <= 0)
            {
                return BadRequest();
            }

            return Ok(new
            {
                Message = "Created successfully",
                Id = result,
                Data = pageDto,
                Pageimageurl = uploadedUrl
            });
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(int id, [FromForm] PageDto.Update pageUpdateDto, IFormFile? pageFile)
        {

            var existing = await _pageService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Không tìm thấy trang truyện cần cập nhật.");
            }

            string finalImageUrl = existing.Pageimageurl;

            if (pageFile != null && pageFile.Length > 0)
            {
                await using var stream = pageFile.OpenReadStream();
                finalImageUrl = await _fileStorage.UploadAsync(
                    stream, pageFile.FileName, pageFile.ContentType, "manga-pages");
            }

            var result = await _pageService.UpdateAsync(id, pageUpdateDto, finalImageUrl);
            if (result <= 0)
            {
                return BadRequest("Cập nhật thất bại.");
            }

            return NoContent();
        }

        [HttpPost("{id:int}/upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, IFormFile pageFile)
        {
            if (pageFile == null || pageFile.Length == 0)
                return BadRequest("Vui lòng chọn hình ảnh trang truyện.");

            await using var stream = pageFile.OpenReadStream();

            var uploadedUrl = await _fileStorage.UploadAsync(
                stream,
                pageFile.FileName,
                pageFile.ContentType,
                "manga-pages"
            );

            var result = await _pageService.UploadImageAsync(id, uploadedUrl);

            if (!result)
                return NotFound("Không tìm thấy page.");

            return Ok(new
            {
                message = "Page image uploaded successfully",
                pageImageUrl = uploadedUrl
            });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, PageDto.UpdateStatus dto)
        {
            var result = await _pageService.UpdateStatusAsync(id, dto);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = "Page status updated successfully"
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _pageService.RemoveAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
