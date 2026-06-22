using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RankingsController : ControllerBase
    {
        private readonly IWeeklyRankingService _weeklyRankingService;

        public RankingsController(IWeeklyRankingService weeklyRankingService)
        {
            _weeklyRankingService = weeklyRankingService;
        }

        [HttpPost("import")]
        [Authorize(Roles = "Admin, EB, Editor")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportWeeklyRanking([FromForm] WeeklyRankingDto.Import dto)
        {
            var userIdClaim = User.FindFirst("userid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var roleIdClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            // Cho phép Role Admin hoặc EB (Editor). Ví dụ Role = 1 (Admin) hoặc 3 (Editor/EB).
            // Tạm thời có thể cho phép những role có quyền, ví dụ Roleid = 1, 2, 3
            // Nếu bạn có quy định riêng, có thể thay đổi ở đây.

            if (dto.ExcelFile == null || dto.ExcelFile.Length == 0)
            {
                return BadRequest("Vui lòng tải lên file Excel.");
            }

            var extension = System.IO.Path.GetExtension(dto.ExcelFile.FileName);
            if (extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest("Chỉ chấp nhận file Excel (.xlsx hoặc .xls).");
            }

            var result = await _weeklyRankingService.ImportWeeklyRankingAsync(userId, dto);
            if (!result)
            {
                return BadRequest("Import thất bại. Vui lòng kiểm tra lại cấu trúc file Excel.");
            }

            return Ok(new { message = "Import bảng xếp hạng thành công." });
        }

        [HttpGet("{issueYear}/{issueNumber}")]
        public async Task<IActionResult> GetRankingsByIssue(int issueYear, int issueNumber)
        {
            var rankings = await _weeklyRankingService.GetRankingsByIssueAsync(issueYear, issueNumber);
            return Ok(rankings);
        }

        [HttpGet("series/{seriesId}")]
        public async Task<IActionResult> GetRankingHistoryForSeries(int seriesId)
        {
            var rankings = await _weeklyRankingService.GetRankingHistoryForSeriesAsync(seriesId);
            return Ok(rankings);
        }
    }
}
