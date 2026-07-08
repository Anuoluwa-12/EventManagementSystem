using EventManagementAPI.DTO;
namespace EventManagement.API.Interface
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync(string? search, string? category,string? location, DateTime? date, decimal? maxPrice);
        Task<bool> BookEventAsync(int userId, int eventId);
    }
}
