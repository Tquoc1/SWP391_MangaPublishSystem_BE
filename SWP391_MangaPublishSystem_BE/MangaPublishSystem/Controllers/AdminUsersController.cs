using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTO;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int? roleId, [FromQuery] string? status)
        {
            var users = await _userService.GetUsersAsync(roleId, status);

            return Ok(new
            {
                data = users
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetail(int id)
        {
            var user = await _userService.GetUserDetailsAsync(id);
            if (user == null) return NotFound("Không tìm thấy User này hoặc User đã bị xóa.");
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto.AdminCreateUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var newId = await _userService.AdminCreateUserAsync(request);
                return CreatedAtAction(nameof(GetUserDetail), new { id = newId }, new { message = "Tạo User thành công", userId = newId });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto.AdminUpdateUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.AdminUpdateUserAsync(id, request);
            if (result <= 0) return NotFound("Không tìm thấy User hoặc lỗi khi cập nhật.");

            return Ok(new { message = "Cập nhật User thành công." });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UserDto.AdminUpdateStatusRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _userService.AdminUpdateStatusAsync(id, request.Status);
            if (!success) return NotFound("Không tìm thấy User.");

            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.AdminSoftDeleteUserAsync(id);
            if (!success) return NotFound("Không tìm thấy User.");

            return Ok(new { message = "Xóa User thành công." });
        }
    }
}
