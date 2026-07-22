using EventManagement.API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers;

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
    public async Task<IActionResult> GetAllEvents(
        string? search,
        string? category,
        string? location,
        DateTime? date,
        decimal? maxPrice)
    {
        var events =
            await _eventService.GetAllEventsAsync(
                search,
                category,
                location,
                date,
                maxPrice
            );

        return Ok(events);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid event ID."
            });
        }

        var eventItem =
            await _eventService.GetEventByIdAsync(id);

        if (eventItem is null)
        {
            return NotFound(new
            {
                message =
                    $"Event with ID {id} was not found."
            });
        }

        return Ok(eventItem);
    }

    [HttpPost("{eventId:int}/book")]
    public async Task<IActionResult> BookEvent(
        int eventId,
        [FromQuery] int userId)
    {
        if (eventId <= 0 || userId <= 0)
        {
            return BadRequest(
                "A valid event ID and user ID are required."
            );
        }

        var result =
            await _eventService.BookEventAsync(
                userId,
                eventId
            );

        if (!result)
        {
            return BadRequest(
                "Unable to book event."
            );
        }

        return Ok(
            "Event booked successfully."
        );
    }
}