using EventManagement.API.Interface;
using EventManagement.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents(string? search, string? category, string? location, DateTime? date, decimal? maxPrice)
        {
            var events = await _eventService.GetAllEventsAsync(
                search,
                category,
                location,
                date,
                maxPrice);

            return Ok(events);
        }

        [HttpPost("{eventId}/book")]
        public async Task<IActionResult> BookEvent(int eventId, int userId)
        {
            var result = await _eventService.BookEventAsync(userId, eventId);

            if (!result)
                return BadRequest("Unable to book event.");

            return Ok("Event booked successfully.");
        }
    }
    }
