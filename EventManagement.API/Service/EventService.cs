using EventManagementAPI.DTO;
using EventManagement.API.Interface;
using EventManagement.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.API.Service
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

    public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            return await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Venue = e.Venue,
                    TicketPrice = e.TicketPrice,
                    Category = e.Category,
                    EventImageUrl = e.EventImageUrl,
                    OrganizerName = e.OrganizerName
                })
                .ToListAsync();
        }
    }
}
