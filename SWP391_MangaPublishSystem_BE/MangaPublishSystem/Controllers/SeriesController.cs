using Entities.Models;
using Services.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesService _seriesService;
        private readonly IFileStorageService _fileStorage;

        public SeriesController(ISeriesService seriesService, IFileStorageService fileStorage)
        {
            _seriesService = seriesService;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<SeriesDto>>> GetAll()
        {
            var series = await _seriesService.GetAllAsync();
            return Ok(series);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SeriesDto>> GetById(int id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null)
            {
                return NotFound();
            }

            return Ok(series);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Create([FromForm] SeriesDto.Create seriesDto, IFormFile proposalFile)
        {
            if (proposalFile == null || proposalFile.Length == 0)
            {
                return BadRequest("Vui lòng đính kèm file bản đề xuất.");
            }
            await using var stream = proposalFile.OpenReadStream();
            string uploadedUrl = await _fileStorage.UploadAsync(
                stream, proposalFile.FileName, proposalFile.ContentType, "proposals");

            var result = await _seriesService.CreateAsync(seriesDto, uploadedUrl);

            if (result <= 0)
            {
                return BadRequest();
            }

            return Ok(new
            {
                Message = "Created successfully",
                Id = result,
                Data = seriesDto,
                Proposalfileurl = uploadedUrl
            });
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(int id, [FromForm] SeriesDto.Update seriesDto, IFormFile? proposalFile)
        {
            if (id != seriesDto.Seriesid)
            {
                return BadRequest("ID route và ID trong dữ liệu không khớp.");
            }

            var existing = await _seriesService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound("Không tìm thấy tác phẩm cần cập nhật.");
            }

            string finalProposalUrl = existing.Proposalfileurl;

            if (proposalFile != null && proposalFile.Length > 0)
            {
                await using var stream = proposalFile.OpenReadStream();

                finalProposalUrl = await _fileStorage.UploadAsync(
                    stream, proposalFile.FileName, proposalFile.ContentType, "proposals");
            }

            var result = await _seriesService.UpdateAsync(seriesDto, finalProposalUrl);
            if (result <= 0)
            {
                return BadRequest("Cập nhật thất bại.");
            }

            return NoContent(); 
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _seriesService.RemoveAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
