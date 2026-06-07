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

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserDto.UpdateProfile request)
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

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest("Fullname is required.");
            }

            user.Fullname = request.FullName;
            await _userService.UpdateUser(user);

            if (user.Roleid == 4)
            {
                if (string.IsNullOrWhiteSpace(request.PenName))
                {
                    return BadRequest("PenName is required.");
                }

                var profile = await _userService.GetMangakaProfile(userId) ?? new MangakaProfile { Userid = userId };
                profile.PenName = request.PenName;
                profile.Bio = request.Bio;
                profile.PhoneNumber = request.PhoneNumber;
                profile.BankName = request.BankName;
                profile.BankAccountNumber = request.BankAccountNumber;
                profile.BankAccountName = request.BankAccountName;

                await _userService.UpsertMangakaProfile(profile);
                return NoContent();
            }

            if (user.Roleid == 5)
            {
                if (string.IsNullOrWhiteSpace(request.Skills) || string.IsNullOrWhiteSpace(request.SoftwareUsed))
                {
                    return BadRequest("Skills and SoftwareUsed are required.");
                }

                if (!request.IsAvailable.HasValue)
                {
                    return BadRequest("IsAvailable is required.");
                }

                var profile = await _userService.GetAssistantProfile(userId) ?? new AssistantProfile { Userid = userId };
                profile.PortfolioUrl = request.PortfolioUrl;
                profile.PhoneNumber = request.PhoneNumber;
                profile.IsAvailable = request.IsAvailable;
                profile.Skills = request.Skills;
                profile.SoftwareUsed = request.SoftwareUsed;
                profile.BankName = request.BankName;
                profile.BankAccountNumber = request.BankAccountNumber;
                profile.BankAccountName = request.BankAccountName;

                await _userService.UpsertAssistantProfile(profile);
                return NoContent();
            }

            return BadRequest("Unsupported role for profile.");
        }
    }
}

