using System.Net.Http.Headers;
using System.Net.Http.Json;
using EventManagementSystem.Interface;
using EventManagementSystem.Models.Admin;

namespace EventManagementSystem.Service;

public sealed class AdminDashboardApiService(
    HttpClient httpClient
) : IAdminDashboardApiService
{
    public async Task<AdminDashboardViewModel?> GetDashboardAsync(
        string accessToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/admin/dashboard"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );

        using var response =
            await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<AdminDashboardViewModel>();
    }
}