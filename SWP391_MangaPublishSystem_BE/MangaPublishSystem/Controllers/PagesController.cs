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
        private readonly IChapterService _chapterService;
        private readonly ISeriesService _seriesService;

        public PagesController(
            IPageService pageService, 
            IPageLayerService pageLayerService, 
            IFileStorageService fileStorage,
            IChapterService chapterService,
            ISeriesService seriesService)
        {
            _pageService = pageService;
            _pageLayerService = pageLayerService;
            _fileStorage = fileStorage;
            _chapterService = chapterService;
            _seriesService = seriesService;
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

            var result = await _pageService.CreateAsync(pageDto);

            if (result <= 0)
            {
                return BadRequest();
            }

            int uploaderId = 0;
            if (User.HasClaim(c => c.Type == "userid"))
            {
                int.TryParse(User.FindFirst("userid")?.Value, out uploaderId);
            }

            // Nếu người dùng không đăng nhập (uploaderId = 0), lấy Mangakaid làm uploaderId mặc định
            if (uploaderId == 0)
            {
                var chapter = await _chapterService.GetByIdAsync(pageDto.Chapterid);
                if (chapter != null)
                {
                    var series = await _seriesService.GetByIdAsync(chapter.Seriesid);
                    if (series != null)
                    {
                        uploaderId = series.Mangakaid;
                    }
                }
            }

            var layerDto = new PageLayerDto.Create
            {
                Pageid = result,
                Uploaderid = uploaderId,
                Layername = "Default",
                Zindex = 1
            };
            
            await _pageLayerService.CreateAsync(layerDto, uploadedUrl);

            return Ok(new
            {
                Message = "Created successfully",
                Id = result,
                Data = pageDto,
                Pageimageurl = uploadedUrl
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
