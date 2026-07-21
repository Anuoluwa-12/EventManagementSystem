using EventManagement.API.Data;
using EventManagement.API.DTO;
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

    /*
     * Main dashboard statistics.
     */
    public async Task<AdminDashboardDto>GetDashboardAsync()
    {
        var totalUsers =
            await _context.Users
                .AsNoTracking()
                .CountAsync(x =>
                    x.Role != "Admin"
                );

        var totalOrganizers =
            await _context.OrganizerProfiles
                .AsNoTracking()
                .CountAsync();

        var totalBookings =
            await _context.EventRegistrations
                .AsNoTracking()
                .CountAsync();

        var totalEvents =
            await _context.Events
                .AsNoTracking()
                .CountAsync();

        var totalRevenue =
            await _context.EventRegistrations
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus == "Paid" &&
                    x.BookingStatus != "Cancelled"
                )
                .SumAsync(x =>
                    (decimal?)x.AmountPaid
                )
            ?? 0m;

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalOrganizers = totalOrganizers,
            TotalBookings = totalBookings,
            TotalEvents = totalEvents,
            TotalRevenue = totalRevenue
        };
    }

    /*
     * All registered users.
     */
    public async Task<List<AdminUserDto>>GetUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => new AdminUserDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                Email = x.Email,
                Role = x.Role
            })
            .ToListAsync();
    }

    /*
     * All organizer profiles.
     */
    public async Task<List<AdminOrganizerDto>>GetOrganizersAsync()
    {
        return await (
            from organizer in
                _context.OrganizerProfiles
                    .AsNoTracking()

            join user in
                _context.Users.AsNoTracking()
                on organizer.UserId equals user.Id

            orderby organizer.Id descending

            select new AdminOrganizerDto
            {
                Id = organizer.Id,

                UserId = organizer.UserId,

                DisplayName =
                    organizer.DisplayName,

                Email = user.Email,

                Phone = organizer.Phone,

                Bio = organizer.Bio,

                Status = organizer.Status,

                CreatedAt =
                    organizer.CreatedAt,

                TotalEvents =
                    _context.Events.Count(x =>
                        x.OrganizerId ==
                        organizer.Id
                    )
            }
        ).ToListAsync();
    }

    /*
     * All platform events.
     */
    public async Task<List<AdminEventDto>>GetEventsAsync()
    {
        return await _context.Events
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => new AdminEventDto
            {
                Id = x.Id,

                Title = x.Title,

                OrganizerName =
                    _context.OrganizerProfiles
                        .Where(organizer =>
                            organizer.Id ==
                            x.OrganizerId
                        )
                        .Select(organizer =>
                            organizer.DisplayName
                        )
                        .FirstOrDefault()
                    ??
                    x.OrganizerName
                    ??
                    "Not assigned",

                EventDate = x.EventDate,

                EventTime = x.EventTime,

                Venue = x.Venue,

                TicketPrice = x.TicketPrice,

                TotalSeats = x.TotalSeats,

                AvailableSeats =
                    x.AvailableSeats,

                Status = x.Status
            })
            .ToListAsync();
    }

    /*
     * All bookings.
     */
    public async Task<List<AdminBookingDto>>GetBookingsAsync()
    {
        return await (
            from registration in
                _context.EventRegistrations
                    .AsNoTracking()

            join eventItem in
                _context.Events.AsNoTracking()
                on registration.EventId
                equals eventItem.Id

            join user in
                _context.Users.AsNoTracking()
                on registration.UserId
                equals user.Id

            orderby registration.Id descending

            select new AdminBookingDto
            {
                Id = registration.Id,

                EventId =
                    registration.EventId,

                EventTitle =
                    eventItem.Title,

                UserId =
                    registration.UserId,

                AttendeeName =
                    user.FirstName,

                AttendeeEmail =
                    user.Email,

                AmountPaid =
                    registration.AmountPaid,

                BookingStatus =
                    registration.BookingStatus,

                PaymentStatus =
                    registration.PaymentStatus
            }
        ).ToListAsync();
    }

    /*
     * Payment records are taken from event
     * registrations because registration currently
     * contains AmountPaid and PaymentStatus.
     */
    public async Task<List<AdminPaymentDto>>GetPaymentsAsync()
    {
        return await (
            from registration in
                _context.EventRegistrations
                    .AsNoTracking()

            join eventItem in
                _context.Events.AsNoTracking()
                on registration.EventId
                equals eventItem.Id

            join user in
                _context.Users.AsNoTracking()
                on registration.UserId
                equals user.Id

            orderby registration.Id descending

            select new AdminPaymentDto
            {
                BookingId =
                    registration.Id,

                EventTitle =
                    eventItem.Title,

                CustomerName =
                    user.FirstName,

                CustomerEmail =
                    user.Email,

                Amount =
                    registration.AmountPaid,

                PaymentStatus =
                    registration.PaymentStatus,

                BookingStatus =
                    registration.BookingStatus
            }
        ).ToListAsync();
    }

    /*
     * Platform report totals.
     */
    public async Task<AdminReportDto>GetReportsAsync()
    {
        var today = DateTime.Today;

        var totalUsers =
            await _context.Users
                .AsNoTracking()
                .CountAsync(x =>
                    x.Role != "Admin"
                );

        var totalOrganizers =
            await _context.OrganizerProfiles
                .AsNoTracking()
                .CountAsync();

        var activeOrganizers =
            await _context.OrganizerProfiles
                .AsNoTracking()
                .CountAsync(x =>
                    x.Status == "Active"
                );

        var totalEvents =
            await _context.Events
                .AsNoTracking()
                .CountAsync();

        var upcomingEvents =
            await _context.Events
                .AsNoTracking()
                .CountAsync(x =>
                    x.EventDate >= today &&
                    x.Status != "Cancelled"
                );

        var totalBookings =
            await _context.EventRegistrations
                .AsNoTracking()
                .CountAsync();

        var paidBookings =
            await _context.EventRegistrations
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus == "Paid" &&
                    x.BookingStatus != "Cancelled"
                );

        var pendingBookings =
            await _context.EventRegistrations
                .AsNoTracking()
                .CountAsync(x =>
                    x.PaymentStatus == "Pending" &&
                    x.BookingStatus != "Cancelled"
                );

        var cancelledBookings =
            await _context.EventRegistrations
                .AsNoTracking()
                .CountAsync(x =>
                    x.BookingStatus == "Cancelled"
                );

        var totalRevenue =
            await _context.EventRegistrations
                .AsNoTracking()
                .Where(x =>
                    x.PaymentStatus == "Paid" &&
                    x.BookingStatus != "Cancelled"
                )
                .SumAsync(x =>
                    (decimal?)x.AmountPaid
                )
            ?? 0m;

        return new AdminReportDto
        {
            TotalUsers = totalUsers,

            TotalOrganizers =
                totalOrganizers,

            ActiveOrganizers =
                activeOrganizers,

            TotalEvents = totalEvents,

            UpcomingEvents =
                upcomingEvents,

            TotalBookings =
                totalBookings,

            PaidBookings =
                paidBookings,

            PendingBookings =
                pendingBookings,

            CancelledBookings =
                cancelledBookings,

            TotalRevenue =
                totalRevenue
        };
    }
}