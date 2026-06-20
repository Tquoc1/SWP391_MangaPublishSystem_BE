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

        [HttpGet("mangaka")]
        [Authorize(Roles = "4")] // 4 = Mangaka
        public async Task<IActionResult> GetMangakaDashboard()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var data = await _dashboardService.GetMangakaDashboardAsync(userId);
            return Ok(data);
        }

        [HttpGet("assistant")]
        [Authorize(Roles = "5")] // 5 = Assistant
        public async Task<IActionResult> GetAssistantDashboard()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var data = await _dashboardService.GetAssistantDashboardAsync(userId);
            return Ok(data);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "1,2,3")] // 1=Admin, 2=Manager, 3=Editor
        public async Task<IActionResult> GetAdminDashboard()
        {
            var data = await _dashboardService.GetAdminDashboardAsync();
            return Ok(data);
        }
    }
}
