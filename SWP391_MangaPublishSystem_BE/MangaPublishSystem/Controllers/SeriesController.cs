using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Services.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<List<SeriesDto>>> GetAll([FromQuery] int? mangakaId, [FromQuery] string? status)
        {
            var series = await _seriesService.GetAllAsync(mangakaId, status);
            return Ok(series);
        }

        [HttpGet("{id:int}", Order = 2)]
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
        [Authorize(Roles = "Mangaka")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Create([FromForm] SeriesDto.Create seriesDto, IFormFile proposalFile, IFormFile coverImage)
        {
      
            if (proposalFile == null || proposalFile.Length == 0)
            {
                return BadRequest("Vui lòng đính kèm file bản đề xuất (PDF/Word).");
            }

            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("Vui lòng đính kèm ảnh bìa của truyện.");
            }

        
            await using var proposalStream = proposalFile.OpenReadStream();
            string proposalUrl = await _fileStorage.UploadAsync(
                proposalStream, proposalFile.FileName, proposalFile.ContentType, "proposals");

            await using var coverStream = coverImage.OpenReadStream();
            string coverImageUrl = await _fileStorage.UploadAsync(
                coverStream, coverImage.FileName, coverImage.ContentType, "covers");


            var result = await _seriesService.CreateAsync(seriesDto, proposalUrl, coverImageUrl);

            if (result <= 0)
            {
                return BadRequest("Tạo mới tác phẩm thất bại.");
            }

            return Ok(new
            {
                Message = "Created successfully",
                Id = result,
                Data = seriesDto,
                Proposalfileurl = proposalUrl,
                Coverimageurl = coverImageUrl
            });
        }

        //[HttpPost("{id:int}/upload-cover")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadCover(int id, IFormFile coverImage)
        //{
        //    if (coverImage == null || coverImage.Length == 0)
        //        return BadRequest("Vui lòng chọn ảnh bìa.");

        //    await using var stream = coverImage.OpenReadStream();

        //    var uploadedUrl = await _fileStorage.UploadAsync(
        //        stream,
        //        coverImage.FileName,
        //        coverImage.ContentType,
        //        "covers"
        //    );

        //    var result = await _seriesService.UploadCoverAsync(id, uploadedUrl);

        //    if (!result)
        //        return NotFound("Không tìm thấy series.");

        //    return Ok(new
        //    {
        //        message = "Cover uploaded successfully",
        //        coverImageUrl = uploadedUrl
        //    });
        //}

        //[HttpPost("{id:int}/upload-proposal")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadProposal(int id, IFormFile proposalFile)
        //{
        //    if (proposalFile == null || proposalFile.Length == 0)
        //        return BadRequest("Vui lòng chọn file proposal.");

        //    await using var stream = proposalFile.OpenReadStream();

        //    var uploadedUrl = await _fileStorage.UploadAsync(
        //        stream,
        //        proposalFile.FileName,
        //        proposalFile.ContentType,
        //        "proposals"
        //    );

        //    var result = await _seriesService.UploadProposalAsync(id, uploadedUrl);

        //    if (!result)
        //        return NotFound("Không tìm thấy series.");

        //    return Ok(new
        //    {
        //        message = "Proposal uploaded successfully",
        //        proposalFileUrl = uploadedUrl
        //    });
        //}

        [HttpPut("{id:int}", Order = 5)]
        [Authorize(Roles = "Mangaka, Assistant")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(int id, [FromForm] SeriesDto.Update seriesDto, IFormFile? proposalFile, IFormFile? coverImage)
        {
            try
            {
                var existing = await _seriesService.GetByIdAsync(id);
                if (existing == null) return NotFound(new { message = "Không tìm thấy tác phẩm cần cập nhật." });

                string finalProposalUrl = existing.Proposalfileurl;
                string finalCoverImageUrl = existing.Coverimageurl;

                if (proposalFile != null && proposalFile.Length > 0)
                {
                    await using var stream = proposalFile.OpenReadStream();
                    finalProposalUrl = await _fileStorage.UploadAsync(
                        stream, proposalFile.FileName, proposalFile.ContentType, "proposals");
                }

                if (coverImage != null && coverImage.Length > 0)
                {
                    await using var stream = coverImage.OpenReadStream();
                    finalCoverImageUrl = await _fileStorage.UploadAsync(
                        stream, coverImage.FileName, coverImage.ContentType, "covers");
                }

                await _seriesService.UpdateAsync(id, seriesDto, finalProposalUrl, finalCoverImageUrl);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/status", Order = 6)]
        [Authorize(Roles = "Admin, EB, Editor,Mangaka")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] SeriesDto.UpdateStatus statusDto)
        {
            try
            {
                await _seriesService.UpdateStatusAsync(id, statusDto);
                return NoContent();
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

        [HttpPatch("{id:int}/publish-format", Order = 7)]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<ActionResult> UpdatePublishFormat(int id, [FromBody] SeriesDto.UpdatePublishFormat formatDto)
        {
            try
            {
                await _seriesService.UpdatePublishFormatAsync(id, formatDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/tantou-editor")]
        [Authorize(Roles = "Mangaka")]
        public async Task<ActionResult> UpdateTantouEditor(int id, [FromBody] SeriesDto.UpdateTantouEditor updateDto)
        {
            try
            {
                await _seriesService.UpdateTantouEditorAsync(id, updateDto.Tantoueditorid);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("softdelete/{id:int}", Order = 8)]
        [Authorize(Roles = "Admin, EB, Mangaka")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            try
            {
                await _seriesService.SoftDeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}