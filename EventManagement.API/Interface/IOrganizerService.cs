using EventManagement.API.DTO;

namespace EventManagement.API.Interface;

public interface IOrganizerService
{
    Task<bool> RegisterAsync(int userId, RegisterOrganizerRequestDto request);

    Task<OrganizerDashboardDto?> GetDashboardAsync(int userId);

    Task<List<OrganizerEventDto>> GetEventsAsync(int userId);

    Task<OrganizerEventDto?> GetEventAsync(int userId, int eventId);
    Task<int?> CreateEventAsync(int userId,OrganizerEventRequestDto request );

    Task<bool> UpdateEventAsync(int userId, int eventId, OrganizerEventRequestDto request  );

    Task<bool> CancelEventAsync(int userId, int eventId);
    Task<bool> DeleteEventAsync(int userId, int eventId);
}