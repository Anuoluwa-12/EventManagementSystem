using EventManagementSystem.DTO;

namespace EventManagementSystem.Interface
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync();
    }
}
