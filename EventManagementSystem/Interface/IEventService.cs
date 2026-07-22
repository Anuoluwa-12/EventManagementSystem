using EventManagementSystem.DTO;

namespace EventManagementSystem.Interface
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync(string? search, string? category, string? location, DateTime? date, decimal? maxPrice);
        Task<bool> BookEventAsync(int userId, int eventId);
        Task<EventDto?> GetByIdAsync(int id);
    }
}
