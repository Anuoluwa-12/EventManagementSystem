using EventManagementSystem.DTO;
using EventManagementSystem.Interface;
using EventManagementSystem.Models.Admin;
using System.Net.Http.Json;

namespace EventManagementSystem.Service;

public class AdminDashboardApiService : IAdminDashboardApiService
{
    private readonly HttpClient _httpClient;

    public AdminDashboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AdminDashboardViewModel?> GetDashboardAsync()

    {
        using var response = await _httpClient.GetAsync(
            "api/AdminDashboard/");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AdminDashboardViewModel>();
    }
}
//public sealed class AdminDashboardApiService(
//    HttpClient httpClient
//) : IAdminDashboardApiService
//{
//    public async Task<AdminDashboardViewModel?> GetDashboardAsync()
//    {
//        using var response = await httpClient.GetAsync(
//            "api/dashboard"
//        );

//        if (!response.IsSuccessStatusCode)
//        {
//            return null;
//        }

//        return await response.Content
//            .ReadFromJsonAsync<AdminDashboardViewModel>();
//    }
//}