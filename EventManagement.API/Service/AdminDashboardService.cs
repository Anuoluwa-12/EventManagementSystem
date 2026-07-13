using EventManagement.API.Data;
using EventManagement.API.Data;
using EventManagement.API.DTO.Admin;
using EventManagement.API.Entity;
using EventManagement.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Api.Service
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync(
            CancellationToken cancellationToken = default)
        {
            var totalUsers = await _context.Users
                .CountAsync(
                    user => user.Role != "Admin",
                    cancellationToken
                );

            var totalOrganizers = await _context.Users
                .CountAsync(
                    user => user.Role == "Organizer",
                    cancellationToken
                );

            var totalBookings = await _context.EventRegistrations
                .CountAsync(cancellationToken);

            var totalEvents = await _context.Events
                .CountAsync(cancellationToken);

            var totalRevenue = await _context.EventRegistrations
                .Where(booking => booking.PaymentStatus == "Paid")
                .SumAsync(
                    booking => (decimal?)booking.AmountPaid,
                    cancellationToken
                ) ?? 0m;

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalOrganizers = totalOrganizers,
                TotalBookings = totalBookings,
                TotalEvents = totalEvents,
                TotalRevenue = totalRevenue
            };
        }
    }
}