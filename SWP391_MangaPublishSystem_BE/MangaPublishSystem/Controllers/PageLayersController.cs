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
            string finalFileUrl = null;

            if (layerFile != null && layerFile.Length > 0)
            {
                await using var stream = layerFile.OpenReadStream();
                finalFileUrl = await _fileStorage.UploadAsync(
                    stream, layerFile.FileName, layerFile.ContentType, "manga-layers");
            }

            try
            {
                await _pageLayerService.UpdateAsync(id, dto, finalFileUrl);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/visibility")]
        [Authorize(Roles = "Mangaka, Assistant")]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            try
            {
                await _pageLayerService.ToggleVisibilityAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}/soft")]
        [Authorize(Roles = "Mangaka, Assistant, Admin")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            try
            {
                await _pageLayerService.SoftDeleteAsync(id);
                return Ok(new { Message = "Soft deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        //[HttpDelete("{id:int}")]
        //[Authorize(Roles = "Mangaka, Admin")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var result = await _pageLayerService.RemoveAsync(id);
        //    if (!result)
        //    {
        //        return NotFound("Không tìm thấy lớp vẽ để xóa hoặc lớp vẽ không tồn tại.");
        //    }

        //    return NoContent();
        //}
    }
}
