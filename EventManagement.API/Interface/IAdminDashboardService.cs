using EventManagement.API.DTO.Admin;

namespace EventManagement.API.Interface
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
    }
}