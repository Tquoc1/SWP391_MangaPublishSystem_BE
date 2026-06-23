using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DTOs;
using Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TagDto>>> GetAll()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TagDto>> GetById(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound(new { Message = "Không tìm thấy thẻ (tag)." });
            }
            return Ok(tag);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, EB")]
        public async Task<ActionResult> Create([FromBody] TagDto.Create dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _tagService.CreateAsync(dto);
            if (id <= 0) return BadRequest(new { Message = "Tạo thẻ thất bại." });

            return Ok(new
            {
                Message = "Created successfully",
                Id = id,
                Data = dto
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, EB")]
        public async Task<ActionResult> Update(int id, [FromBody] TagDto.Update dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _tagService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, EB")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _tagService.RemoveAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
