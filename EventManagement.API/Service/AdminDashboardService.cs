using EventManagement.API.Data;
using EventManagement.API.DTO.Admin;
using EventManagement.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.API.Service;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        var totalUsers = await _context.Users.CountAsync();

        var totalOrganizers = await _context.Companies.CountAsync();

        var totalBookings =
            await _context.EventRegistrations.CountAsync();

        var totalEvents = await _context.Events.CountAsync();

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalOrganizers = totalOrganizers,
            TotalBookings = totalBookings,
            TotalEvents = totalEvents,
            TotalRevenue = 0m
        };
    }
}