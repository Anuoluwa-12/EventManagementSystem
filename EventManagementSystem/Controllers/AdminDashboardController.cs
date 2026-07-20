using EventManagementSystem.Interface;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class AdminDashboardController : Controller
{
    private readonly IAdminDashboardApiService _dashboardService;

    public AdminDashboardController(IAdminDashboardApiService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var dashboard =
            await _dashboardService
                .GetDashboardAsync(token)
            ?? new AdminDashboardViewModel();

        return View(dashboard);
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var users =
            await _dashboardService
                .GetUsersAsync(token);

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Organizers()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var organizers =
            await _dashboardService
                .GetOrganizersAsync(token);

        return View(organizers);
    }

    [HttpGet]
    public async Task<IActionResult> Events()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var events =
            await _dashboardService
                .GetEventsAsync(token);

        return View(events);
    }

    [HttpGet]
    public async Task<IActionResult> Bookings()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var bookings =
            await _dashboardService
                .GetBookingsAsync(token);

        return View(bookings);
    }

    [HttpGet]
    public async Task<IActionResult> Payments()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var payments =
            await _dashboardService
                .GetPaymentsAsync(token);

        return View(payments);
    }

    [HttpGet]
    public async Task<IActionResult> Reports()
    {
        var token = GetAccessToken();

        if (!IsAdmin() || token is null)
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var reports =
            await _dashboardService
                .GetReportsAsync(token)
            ?? new AdminReportViewModel();

        return View(reports);
    }

    [HttpGet]
    public IActionResult Settings()
    {
        if (!IsAdmin())
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        return View();
    }

    private bool IsAdmin()
    {
        var userRole =
            HttpContext.Session.GetString(
                "Role"
            );

        return string.Equals(
            userRole?.Trim(),
            "Admin",
            StringComparison.OrdinalIgnoreCase
        );
    }

    private string? GetAccessToken()
    {
        var token =
            HttpContext.Session.GetString(
                "Token"
            );

        return string.IsNullOrWhiteSpace(token)
            ? null
            : token;
    }
}