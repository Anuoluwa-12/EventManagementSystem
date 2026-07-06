using EventManagementSystem.DTO;

namespace EventManagementSystem.Interface
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync();
        Task<bool> BookEventAsync(int userId, int eventId);
    }
}
