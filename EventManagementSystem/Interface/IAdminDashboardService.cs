using EventManagementSystem.Models.Admin;

namespace EventManagementSystem.Interface;

public interface IAdminDashboardApiService
{
    Task<AdminDashboardViewModel?> GetDashboardAsync(
        string accessToken
    );
}