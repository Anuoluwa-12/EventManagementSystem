using EventManagement.API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _adminService;

    public AdminDashboardController(IAdminDashboardService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result =
            await _adminService
                .GetDashboardAsync();

        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result =
            await _adminService
                .GetUsersAsync();

        return Ok(result);
    }

    [HttpGet("organizers")]
    public async Task<IActionResult> GetOrganizers()
    {
        var result =
            await _adminService
                .GetOrganizersAsync();

        return Ok(result);
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents()
    {
        var result =
            await _adminService
                .GetEventsAsync();

        return Ok(result);
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings()
    {
        var result =
            await _adminService
                .GetBookingsAsync();

        return Ok(result);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments()
    {
        var result =
            await _adminService
                .GetPaymentsAsync();

        return Ok(result);
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReports()
    {
        var result =
            await _adminService
                .GetReportsAsync();

        return Ok(result);
    }
}