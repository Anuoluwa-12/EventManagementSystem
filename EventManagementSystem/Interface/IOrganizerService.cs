using EventManagementSystem.Models;

namespace EventManagementSystem.Interface;
public interface IOrganizerService
{
Task<bool> RegisterAsync(string token, OrganizerRegistrationViewModel model);
Task<OrganizerDashboardViewModel?> GetDashboardAsync(string token);
Task<List<OrganizerEventViewModel>> GetEventsAsync(string token);
Task<OrganizerEventViewModel?> GetEventAsync(string token, int eventId);
Task<bool> CreateEventAsync(string token, OrganizerEventFormViewModel model);
Task<bool> UpdateEventAsync(string token, int eventId, OrganizerEventFormViewModel model);
Task<bool> CancelEventAsync(string token, int eventId);
Task<bool> DeleteEventAsync(string token, int eventId);
}