using EventManagement.API.DTO;

namespace EventManagement.API.Interface;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(int userId);
}