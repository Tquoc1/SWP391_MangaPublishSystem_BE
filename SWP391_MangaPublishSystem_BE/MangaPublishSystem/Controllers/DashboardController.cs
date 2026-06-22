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
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("userid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }

        [HttpGet("TopSeries")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopSeries()
        {
            var data = await _dashboardService.GetTopSeriesAsync();
            return Ok(data);
        }

        [HttpGet("Admin/Overview")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> GetAdminOverview()
        {
            var data = await _dashboardService.GetAdminOverviewAsync();
            return Ok(data);
        }

        [HttpGet("Admin/SeriesStats")]
        [Authorize(Roles = "Admin, EB, Editor")]
        public async Task<IActionResult> GetAdminSeriesStats()
        {
            var data = await _dashboardService.GetAdminSeriesStatsAsync();
            return Ok(data);
        }
    }
}
