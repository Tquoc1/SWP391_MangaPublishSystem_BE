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
        private readonly IFileStorageService _fileStorage;

        public UsersController(IUserService userService, IFileStorageService fileStorage)
        {
            _userService = userService;
            _fileStorage = fileStorage;
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMangakaProfile([FromForm] UserDto.UpdateMangakaProfile request, IFormFile? avatarFile)
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

            string? finalAvatarUrl = null;
            if (avatarFile != null && avatarFile.Length > 0)
            {
                await using var stream = avatarFile.OpenReadStream();
                finalAvatarUrl = await _fileStorage.UploadAsync(
                    stream, avatarFile.FileName, avatarFile.ContentType, "avatars");
            }

            var result = await _userService.UpdateMangakaProfile(userId, request, finalAvatarUrl);
            if (result <= 0)
            {
                return BadRequest("Failed to update Mangaka profile.");
            }

            return NoContent();
        }

        [HttpPut("profile/assistant")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAssistantProfile([FromForm] UserDto.UpdateAssistantProfile request, IFormFile? avatarFile)
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

            string? finalAvatarUrl = null;
            if (avatarFile != null && avatarFile.Length > 0)
            {
                await using var stream = avatarFile.OpenReadStream();
                finalAvatarUrl = await _fileStorage.UploadAsync(
                    stream, avatarFile.FileName, avatarFile.ContentType, "avatars");
            }

            var result = await _userService.UpdateAssistantProfile(userId, request, finalAvatarUrl);
            if (result <= 0)
            {
                return BadRequest("Failed to update Assistant profile.");
            }

            return NoContent();
        }
    }
}

