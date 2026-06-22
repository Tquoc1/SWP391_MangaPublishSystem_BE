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
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> GetAll()
        {
            var genres = await _genreService.GetAllAsync();
            return Ok(genres);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDto>> GetById(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound(new { Message = "Không tìm thấy thể loại." });
            }
            return Ok(genre);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, EB")]
        public async Task<ActionResult> Create([FromBody] GenreDto.Create dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _genreService.CreateAsync(dto);
            if (id <= 0) return BadRequest(new { Message = "Tạo thể loại thất bại." });

            return Ok(new
            {
                Message = "Created successfully",
                Id = id,
                Data = dto
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, EB")]
        public async Task<ActionResult> Update(int id, [FromBody] GenreDto.Update dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _genreService.UpdateAsync(id, dto);
            if (!result) return NotFound(new { Message = "Không tìm thấy thể loại để cập nhật." });

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, EB")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _genreService.RemoveAsync(id);
            if (!result) return NotFound(new { Message = "Không tìm thấy thể loại để xóa." });

            return NoContent();
        }
    }
}
