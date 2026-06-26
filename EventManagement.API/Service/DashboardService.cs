using EventManagement.API.Data;
using EventManagement.API.DTO;
using EventManagement.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.API.Service;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        return new DashboardDto
        {
            FirstName = user.FirstName,
            UpcomingEvents = 0,
            RegisteredEvents = 0,
            NotificationsCount = 0
        };
    }
}