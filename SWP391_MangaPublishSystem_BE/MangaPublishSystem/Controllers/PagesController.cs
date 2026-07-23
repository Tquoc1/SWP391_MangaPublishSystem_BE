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
        public async Task<ActionResult<List<PageDto>>> GetAll([FromQuery] int? chapterId, [FromQuery] bool? isSentToMangaka, [FromQuery] int? mangakaId)
        {
            if (User.IsInRole("Mangaka"))
            {
                if (User.HasClaim(c => c.Type == "userid"))
                {
                    if (int.TryParse(User.FindFirst("userid")?.Value, out int userId))
                    {
                        mangakaId = userId;
                    }
                }
            }

            var pages = await _pageService.GetAllAsync(chapterId, isSentToMangaka, mangakaId);
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
        [Authorize(Roles = "Mangaka, Assistant")]
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
        [Authorize(Roles = "Mangaka, Assistant")]
        public async Task<ActionResult> Update(int id, [FromBody] PageDto.Update pageUpdateDto)
        {
            try
            {
                await _pageService.UpdateAsync(id, pageUpdateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("{id:int}/composite")]
        [Authorize(Roles = "Mangaka, Assistant")]
        public async Task<ActionResult> CompositeImage(int id)
        {
            try
            {
                var result = await _pageService.CompositeAndSaveImageAsync(id);
                return Ok(new { Message = "Composite successfully", Pageimageurl = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/is-sent-to-mangaka")]
        [Authorize(Roles = "Admin, EB, Editor, Mangaka,Assistant")]
        public async Task<ActionResult> UpdateIsSentToMangaka(int id, [FromBody] bool isSentToMangaka)
        {
            try
            {
                await _pageService.UpdateIsSentToMangakaAsync(id, isSentToMangaka);
                return Ok(new { Message = "IsSentToMangaka updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}/soft")]
        [Authorize(Roles = "Admin, EB, Mangaka")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            try
            {
                await _pageService.SoftDeleteAsync(id);
                return Ok(new { Message = "Soft deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
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
