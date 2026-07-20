using EventManagement.API.DTO;

namespace EventManagement.API.Interface;

public interface IAdminDashboardService
{
    Task<AdminDashboardDto> GetDashboardAsync();

    Task<List<AdminUserDto>> GetUsersAsync();

    Task<List<AdminOrganizerDto>> GetOrganizersAsync();

    Task<List<AdminEventDto>> GetEventsAsync();

    Task<List<AdminBookingDto>> GetBookingsAsync();

    Task<List<AdminPaymentDto>> GetPaymentsAsync();

    Task<AdminReportDto> GetReportsAsync();
}