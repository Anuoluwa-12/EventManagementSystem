using EventManagementSystem.DTO;

namespace EventManagementSystem.Interface
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(int userId);
    }
}