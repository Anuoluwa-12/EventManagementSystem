using EventManagement.API.Data;
using EventManagement.API.Entity;
using EventManagement.API.Interface;
using EventManagementAPI.DTO;
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

        public async Task<List<EventDto>> GetAllEventsAsync(string? search, string? category, string? location, DateTime? date,decimal? maxPrice)
        {
            var query = _context.Events.AsQueryable();

            // Search by title or description
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Title.Contains(search) ||
                    e.Description.Contains(search));
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => e.Category == category);
            }

            // Filter by location (venue)
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(e => e.Venue.Contains(location));
            }

            // Filter by date
            if (date.HasValue)
            {
                query = query.Where(e => e.EventDate.Date == date.Value.Date);
            }

            // Filter by maximum ticket price
            if (maxPrice.HasValue)
            {
                query = query.Where(e => e.TicketPrice <= maxPrice.Value);
            }

            return await query
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

        public async Task<bool> BookEventAsync(int userId, int eventId)
        {
            // Check that the event exists
            var ev = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
                return false;

            // Check if the user has already booked the event
            var alreadyBooked = await _context.EventRegistrations
                .AnyAsync(r => r.UserId == userId && r.EventId == eventId);

            if (alreadyBooked)
                return false;

            // Create the booking
            var registration = new EventRegistration
            {
                UserId = userId,
                EventId = eventId,
                RegistrationDate = DateTime.Now
            };

            _context.EventRegistrations.Add(registration);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    eventItem => eventItem.Id == id
                );
        }
    }
}
