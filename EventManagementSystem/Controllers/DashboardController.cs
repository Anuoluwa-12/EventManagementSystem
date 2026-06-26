using EventManagementSystem.Interface;
using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        int userId = 1; 

        var dashboard =
            await _dashboardService.GetDashboardAsync(userId);

        return View(dashboard);
    }
}