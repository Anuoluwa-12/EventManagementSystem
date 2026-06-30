using EventManagementAPI.DTO;
namespace EventManagement.API.Interface
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync();
    }
}
