using Microsoft.AspNetCore.Mvc;
using EventManagement.API.Interface;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDashboard(int userId)
    {
        var dashboard = await _dashboardService.GetDashboardAsync(userId);

        return Ok(dashboard);
    }
}