using EventManagementSystem.DTO;
using EventManagementSystem.Interface;
using System.Net.Http.Json;

namespace EventManagementSystem.Service
{
    public class EventService : IEventService
    {
        private readonly HttpClient _httpClient;
    public EventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EventDto>> GetAllEventsAsync(string? search, string? category,string? location, DateTime? date, decimal? maxPrice)
        {
            string url = $"https://localhost:7053/api/Event?" +
                         $"search={search}" +
                         $"&category={category}" +
                         $"&location={location}" +
                         $"&date={date:yyyy-MM-dd}" +
                         $"&maxPrice={maxPrice}";

            return await _httpClient.GetFromJsonAsync<List<EventDto>>(url);
        }
        public async Task<bool> BookEventAsync(int userId, int eventId)
        {
            var response = await _httpClient.PostAsync(
                $"https://localhost:7053/api/Event/{eventId}/book?userId={userId}",
                null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsEventBookedAsync(int userId, int eventId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<bool>(
                    $"https://localhost:7053/api/Event/{eventId}/is-booked?userId={userId}");
            }
            catch
            {
                return false;
            }
        }
    }
}
