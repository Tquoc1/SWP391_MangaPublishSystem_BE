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
        [HttpGet(Order = 1)]
        public async Task<ActionResult<List<SeriesDto>>> GetAll()
        {
            var series = await _seriesService.GetAllAsync();
            return Ok(series);
        }

        [HttpGet("{id:int}" , Order = 2)]
        public async Task<ActionResult<SeriesDto>> GetById(int id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null)
            {
                return NotFound();
            }

            return Ok(series);
        }

        [HttpGet("mangakaid/{mangakaId:int}", Order = 3)]
        public async Task<ActionResult<List<SeriesDto>>> GetByMangakaId(int mangakaId)
        {
            var series = await _seriesService.GetByMangakaIdAsync(mangakaId);

            return Ok(series);
        }

        [HttpPost(Order = 4)]
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

        [HttpPut("{id:int}", Order = 5)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(int id, [FromForm] SeriesDto.Update seriesDto, IFormFile? proposalFile)
        {

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

            var result = await _seriesService.UpdateAsync(id, seriesDto, finalProposalUrl);
            if (!result)
            {
                return BadRequest("Cập nhật thất bại.");
            }

            return NoContent(); 
        }


        [HttpPatch("{id:int}/status", Order = 6)]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] SeriesDto.UpdateStatus statusDto)
        {
            var result = await _seriesService.UpdateStatusAsync(id, statusDto);
            if (!result)
            {
                return NotFound("Không tìm thấy tác phẩm hoặc cập nhật trạng thái thất bại.");
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/publish-format", Order = 7)]
        public async Task<ActionResult> UpdatePublishFormat(int id, [FromBody] SeriesDto.UpdatePublishFormat formatDto)
        {
            var result = await _seriesService.UpdatePublishFormatAsync(id, formatDto);
            if (!result)
            {
                return NotFound("Không tìm thấy tác phẩm hoặc cập nhật định dạng thất bại.");
            }

            return NoContent();
        }

        [HttpDelete("softdelete/{id:int}", Order = 8)]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var result = await _seriesService.SoftDeleteAsync(id);
            if (!result)
            {
                return NotFound("Không tìm thấy tác phẩm cần xóa.");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Order = 9)]
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
