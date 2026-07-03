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
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var dashboard = await _dashboardService.GetDashboardAsync(userId.Value);

        return View(dashboard);
    }
}