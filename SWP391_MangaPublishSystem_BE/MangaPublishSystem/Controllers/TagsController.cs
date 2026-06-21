using Microsoft.AspNetCore.Mvc;
using DTOs;
using Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult> Update(int id, [FromBody] TagDto.Update dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _tagService.UpdateAsync(id, dto);
            if (!result) return NotFound(new { Message = "Không tìm thấy thẻ để cập nhật." });

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _tagService.RemoveAsync(id);
            if (!result) return NotFound(new { Message = "Không tìm thấy thẻ để xóa." });

            return NoContent();
        }
    }
}
