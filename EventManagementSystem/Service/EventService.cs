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

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<EventDto>>
                ("https://localhost:7053/api/Event");
        }
    }
}
