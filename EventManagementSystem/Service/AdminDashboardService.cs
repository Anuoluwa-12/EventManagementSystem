using EventManagementSystem.Interface;
using EventManagementSystem.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EventManagementSystem.Service;

public class AdminDashboardApiService : IAdminDashboardApiService
{
    private readonly HttpClient _httpClient;

    public AdminDashboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<AdminDashboardViewModel?>GetDashboardAsync(string token)
    {
        return GetAsync<AdminDashboardViewModel>(
            "api/admin/dashboard",
            token
        );
    }

    public async Task<List<AdminUserViewModel>>GetUsersAsync(string token)
    {
        return await GetAsync<
                List<AdminUserViewModel>>(
                "api/admin/users",
                token
            )
            ?? new List<AdminUserViewModel>();
    }

    public async Task<List<AdminOrganizerViewModel>>GetOrganizersAsync(string token)
    {
        return await GetAsync<
                List<AdminOrganizerViewModel>>(
                "api/admin/organizers",
                token
            )
            ?? new List<
                AdminOrganizerViewModel>();
    }

    public async Task<List<AdminEventViewModel>>GetEventsAsync(string token)
    {
        return await GetAsync<
                List<AdminEventViewModel>>(
                "api/admin/events",
                token
            )
            ?? new List<AdminEventViewModel>();
    }

    public async Task<List<AdminBookingViewModel>>GetBookingsAsync(string token)
    {
        return await GetAsync<
                List<AdminBookingViewModel>>(
                "api/admin/bookings",
                token
            )
            ?? new List<
                AdminBookingViewModel>();
    }

    public async Task<List<AdminPaymentViewModel>>GetPaymentsAsync(string token)
    {
        return await GetAsync<
                List<AdminPaymentViewModel>>(
                "api/admin/payments",
                token
            )
            ?? new List<
                AdminPaymentViewModel>();
    }

    public Task<AdminReportViewModel?>GetReportsAsync(string token)
    {
        return GetAsync<AdminReportViewModel>(
            "api/admin/reports",
            token
        );
    }

    private async Task<T?> GetAsync<T>(string requestUrl, string token)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                requestUrl
            );

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token
            );

        using var response =
            await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content
            .ReadFromJsonAsync<T>();
    }
}