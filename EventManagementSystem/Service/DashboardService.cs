using EventManagementSystem.Interface;
using EventManagementSystem.DTO;

namespace EventManagementSystem.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardDto> GetDashboardAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<DashboardDto>(
                $"https://localhost:7053/api/Dashboard/{userId}");
        }
    }
}
