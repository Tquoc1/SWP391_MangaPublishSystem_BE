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
    public class PageLayersController : ControllerBase
    {
        private readonly IPageLayerService _pageLayerService;
        private readonly IFileStorageService _fileStorage;

        public PageLayersController(IPageLayerService pageLayerService, IFileStorageService fileStorage)
        {
            _pageLayerService = pageLayerService;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<PageLayerDto>>> GetAll([FromQuery] int? pageId)
        {
            var layers = await _pageLayerService.GetAllAsync(pageId);
            return Ok(layers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PageLayerDto>> GetById(int id)
        {
            var layer = await _pageLayerService.GetByIdAsync(id);
            if (layer == null)
            {
                return NotFound("Không tìm thấy lớp vẽ (Layer) yêu cầu.");
            }
            return Ok(layer);
        }

        [HttpPost]
        [Authorize(Roles = "Mangaka, Assistant")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Create([FromForm] PageLayerDto.Create dto, IFormFile layerFile)
        {
            if (layerFile == null || layerFile.Length == 0)
            {
                return BadRequest("Vui lòng đính kèm file bản vẽ (ảnh hoặc file gốc thiết kế) cho Layer.");
            }

            await using var stream = layerFile.OpenReadStream();
            string uploadedUrl = await _fileStorage.UploadAsync(
                stream, layerFile.FileName, layerFile.ContentType, "manga-layers");

            var resultId = await _pageLayerService.CreateAsync(dto, uploadedUrl);

            if (resultId <= 0)
            {
                return BadRequest("Tạo mới lớp vẽ thất bại.");
            }

            return Ok(new
            {
                Message = "Created successfully",
                Id = resultId,
                Data = dto,
                Fileurl = uploadedUrl
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Mangaka, Assistant")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(int id, [FromForm] PageLayerDto.Update dto, IFormFile? layerFile)
        {
            var existing = await _pageLayerService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Không tìm thấy lớp vẽ (Layer) cần cập nhật.");
            }

            string finalFileUrl = existing.Fileurl;

            if (layerFile != null && layerFile.Length > 0)
            {
                await using var stream = layerFile.OpenReadStream();
                finalFileUrl = await _fileStorage.UploadAsync(
                    stream, layerFile.FileName, layerFile.ContentType, "manga-layers");
            }

            var result = await _pageLayerService.UpdateAsync(id, dto, finalFileUrl);
            if (result <= 0)
            {
                return BadRequest("Cập nhật lớp vẽ thất bại.");
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/visibility")]
        [Authorize(Roles = "Mangaka, Assistant")]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            var result = await _pageLayerService.ToggleVisibilityAsync(id);
            if (!result) return NotFound("Không tìm thấy lớp nhân vật hoặc trang truyện.");
            return NoContent();
        }

        [HttpDelete("{id:int}/soft")]
        [Authorize(Roles = "Mangaka, Assistant, Admin")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var success = await _pageLayerService.SoftDeleteAsync(id);
            if (!success)
            {
                return NotFound("Không tìm thấy lớp vẽ để xóa tạm.");
            }
            return Ok(new { Message = "Soft deleted successfully" });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Mangaka, Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _pageLayerService.RemoveAsync(id);
            if (!result)
            {
                return NotFound("Không tìm thấy lớp vẽ để xóa hoặc lớp vẽ không tồn tại.");
            }

            return NoContent();
        }
    }
}
