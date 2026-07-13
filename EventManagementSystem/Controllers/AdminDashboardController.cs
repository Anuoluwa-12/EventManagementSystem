using EventManagementSystem.Interface;
using EventManagementSystem.Models.Admin;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class AdminDashboardController(
    IAdminDashboardApiService dashboardService
) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userRole =
            HttpContext.Session.GetString("UserRole");

        var accessToken =
            HttpContext.Session.GetString("AccessToken");

        if (!string.Equals(
            userRole?.Trim(),
            "Admin",
            StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return RedirectToAction("Login", "Auth");
        }

        var dashboard =
            await dashboardService.GetDashboardAsync(accessToken);

        if (dashboard is null)
        {
            ViewBag.ErrorMessage =
                "The admin dashboard could not be loaded.";

            dashboard = new AdminDashboardViewModel();
        }

        return View(dashboard);
    }
}