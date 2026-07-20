using EventManagementSystem.Models;

namespace EventManagementSystem.Interface;

public interface IAdminDashboardApiService
{
    Task<AdminDashboardViewModel?>GetDashboardAsync(string token);

    Task<List<AdminUserViewModel>>GetUsersAsync(string token);

    Task<List<AdminOrganizerViewModel>>GetOrganizersAsync(string token);

    Task<List<AdminEventViewModel>>GetEventsAsync(string token);

    Task<List<AdminBookingViewModel>>GetBookingsAsync(string token);

    Task<List<AdminPaymentViewModel>>GetPaymentsAsync(string token);

    Task<AdminReportViewModel?>GetReportsAsync(string token);
}