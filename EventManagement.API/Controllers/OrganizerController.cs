using EventManagement.API.DTO;
using EventManagement.API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventManagement.API.Controllers;

[ApiController]
[Route("api/organizer")]
[Authorize]
public class OrganizerController : ControllerBase
{
    private readonly IOrganizerService _organizerService;

    public OrganizerController(
        IOrganizerService organizerService)
    {
        _organizerService = organizerService;
    }

    /*
     * ==================================================
     * REGISTER AS AN ORGANIZER
     * POST: api/organizer/register
     * ==================================================
     */

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterOrganizerRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _organizerService.RegisterAsync(
                userId.Value,
                request
            );

            return Ok(new
            {
                message =
                    "Organizer registration completed successfully."
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine(
                $"Organizer database error: {exception.InnerException?.Message ?? exception.Message}"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "The organizer profile could not be saved because of a database error."
                }
            );
        }
    }
    /*
     * ==================================================
     * ORGANIZER DASHBOARD
     * GET: api/organizer/dashboard
     * ==================================================
     */

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        var dashboard =
            await _organizerService.GetDashboardAsync(
                userId.Value
            );

        if (dashboard is null)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message =
                        "You do not have an active organizer profile."
                }
            );
        }

        return Ok(dashboard);
    }

    /*
     * ==================================================
     * GET ALL EVENTS CREATED BY THE ORGANIZER
     * GET: api/organizer/events
     * ==================================================
     */

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        var events =
            await _organizerService.GetEventsAsync(
                userId.Value
            );

        return Ok(events);
    }

    /*
     * ==================================================
     * GET ONE EVENT
     * GET: api/organizer/events/5
     * ==================================================
     */

    [HttpGet("events/{eventId:int}")]
    public async Task<IActionResult> GetEvent(
        int eventId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        if (eventId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid event ID is required."
            });
        }

        var eventItem =
            await _organizerService.GetEventAsync(
                userId.Value,
                eventId
            );

        if (eventItem is null)
        {
            return NotFound(new
            {
                message =
                    "The event was not found or does not belong to this organizer."
            });
        }

        return Ok(eventItem);
    }

    /*
     * ==================================================
     * CREATE AN EVENT
     * POST: api/organizer/events
     * ==================================================
     */

    [HttpPost("events")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> CreateEvent([FromForm] OrganizerEventRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var eventId =
                await _organizerService.CreateEventAsync(
                    userId.Value,
                    request
                );

            if (eventId is null)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You must have an active organizer profile before creating an event."
                    }
                );
            }

            return Ok(new
            {
                eventId = eventId.Value,
                message =
                    "The event was created successfully."
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    /*
     * ==================================================
     * UPDATE AN EVENT
     * PUT: api/organizer/events/5
     * ==================================================
     */

    [HttpPut("events/{eventId:int}")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> UpdateEvent(int eventId, [FromForm] OrganizerEventRequestDto request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        if (eventId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid event ID is required."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated =
                await _organizerService.UpdateEventAsync(
                    userId.Value,
                    eventId,
                    request
                );

            if (!updated)
            {
                return NotFound(new
                {
                    message =
                        "The event was not found, does not belong to you, or cannot be edited."
                });
            }

            return Ok(new
            {
                message =
                    "The event was updated successfully."
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    /*
     * ==================================================
     * CANCEL AN EVENT
     * PATCH: api/organizer/events/5/cancel
     * ==================================================
     */

    [HttpPatch("events/{eventId:int}/cancel")]
    public async Task<IActionResult> CancelEvent(int eventId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        if (eventId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid event ID is required."
            });
        }

        var cancelled =
            await _organizerService.CancelEventAsync(
                userId.Value,
                eventId
            );

        if (!cancelled)
        {
            return NotFound(new
            {
                message =
                    "The event was not found or does not belong to this organizer."
            });
        }

        return Ok(new
        {
            message =
                "The event was cancelled successfully."
        });
    }

    /*
     * ==================================================
     * DELETE AN EVENT
     * DELETE: api/organizer/events/5
     * ==================================================
     */

    [HttpDelete("events/{eventId:int}")]
    public async Task<IActionResult> DeleteEvent(int eventId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                message = "The authenticated user ID is missing."
            });
        }

        if (eventId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid event ID is required."
            });
        }

        var deleted =
            await _organizerService.DeleteEventAsync(
                userId.Value,
                eventId
            );

        if (!deleted)
        {
            return BadRequest(new
            {
                message =
                    "The event could not be deleted. It may have registrations, may not exist, or may not belong to you. Cancel it instead if bookings already exist."
            });
        }

        return Ok(new
        {
            message =
                "The event was deleted successfully."
        });
    }

    /*
     * ==================================================
     * GET LOGGED-IN USER ID FROM JWT
     * ==================================================
     */

    private int? GetCurrentUserId()
    {
        var userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        if (!int.TryParse(
            userIdValue,
            out var userId))
        {
            return null;
        }

        return userId;
    }
}