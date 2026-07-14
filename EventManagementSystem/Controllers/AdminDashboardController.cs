using EventManagementSystem.Interface;
using EventManagementSystem.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class AdminDashboardController : Controller
{
    private readonly IAdminDashboardApiService _dashboardService;

    public AdminDashboardController(
        IAdminDashboardApiService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var userRole =
            HttpContext.Session.GetString("Role");

        if (!string.Equals(
            userRole?.Trim(),
            "Admin",
            StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        var dashboard =
            await _dashboardService.GetDashboardAsync()
            ?? new AdminDashboardViewModel();

        return View(dashboard);
    }
}