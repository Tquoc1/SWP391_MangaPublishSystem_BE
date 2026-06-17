using System.Security.Claims;
using Entities.Models;
using Services.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace MangaPublishSystem.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst("userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Roleid == 4)
            {
                var profile = await _userService.GetMangakaProfile(userId);
                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    user.Userid,
                    user.Username,
                    user.Fullname,
                    user.Email,
                    PenName = profile.PenName,
                    AvatarUrl = profile.AvatarUrl,
                    Bio = profile.Bio,
                    PhoneNumber = profile.PhoneNumber,
                    BankName = profile.BankName,
                    BankAccountNumber = profile.BankAccountNumber,
                    BankAccountName = profile.BankAccountName
                });
            }

            if (user.Roleid == 5)
            {
                var profile = await _userService.GetAssistantProfile(userId);
                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    user.Userid,
                    user.Username,
                    user.Fullname,
                    user.Email,
                    AvatarUrl = profile.AvatarUrl,
                    PortfolioUrl = profile.PortfolioUrl,
                    PhoneNumber = profile.PhoneNumber,
                    IsAvailable = profile.IsAvailable,
                    Skills = profile.Skills,
                    SoftwareUsed = profile.SoftwareUsed,
                    BankName = profile.BankName,
                    BankAccountNumber = profile.BankAccountNumber,
                    BankAccountName = profile.BankAccountName
                });
            }

            return BadRequest("Unsupported role for profile.");
        }

        [HttpPut("profile/mangaka")]
        public async Task<IActionResult> UpdateMangakaProfile([FromBody] UserDto.UpdateMangakaProfile request)
        {
            var userIdClaim = User.FindFirst("userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userId);
            if (user == null || user.Roleid != 4)
            {
                return BadRequest("User not found or is not a Mangaka.");
            }

            var result = await _userService.UpdateMangakaProfile(userId, request);
            if (result <= 0)
            {
                return BadRequest("Failed to update Mangaka profile.");
            }

            return NoContent();
        }

        [HttpPut("profile/assistant")]
        public async Task<IActionResult> UpdateAssistantProfile([FromBody] UserDto.UpdateAssistantProfile request)
        {
            var userIdClaim = User.FindFirst("userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userId);
            if (user == null || user.Roleid != 5)
            {
                return BadRequest("User not found or is not an Assistant.");
            }

            var result = await _userService.UpdateAssistantProfile(userId, request);
            if (result <= 0)
            {
                return BadRequest("Failed to update Assistant profile.");
            }

            return NoContent();
        }
    }
}

