using EventManagementSystem.Interface;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class OrganizerController : Controller
{
    private readonly IOrganizerService _organizerService;

    public OrganizerController(
        IOrganizerService organizerService)
    {
        _organizerService = organizerService;
    }

    /*
     * ==================================================
     * ORGANIZER REGISTRATION
     * ==================================================
     */

    [HttpGet]
    public IActionResult Register()
    {
        if (!IsLoggedIn())
        {
            TempData["Error"] =
                "Please log in before registering as an organizer.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        OrganizerRegistrationViewModel model)
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _organizerService.RegisterAsync(
                token,
                model
            );

            TempData["OrganizerMessage"] =
                "Your organizer profile was created successfully.";

            return RedirectToAction(
                nameof(Dashboard)
            );
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message
            );

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine(
                $"Organizer API connection error: {exception}"
            );

            ModelState.AddModelError(
                string.Empty,
                "The organizer service could not be reached."
            );

            return View(model);
        }
        catch (Exception exception)
        {
            Console.WriteLine(
                $"Unexpected organizer registration error: {exception}"
            );

            ModelState.AddModelError(
                string.Empty,
                "An unexpected error occurred while creating the organizer profile."
            );

            return View(model);
        }
    }

    /*
     * ==================================================
     * ORGANIZER DASHBOARD
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Please log in before accessing the organizer dashboard.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        try
        {
            var dashboard =
                await _organizerService
                    .GetDashboardAsync(token);

            if (dashboard is null)
            {
                TempData["OrganizerError"] =
                    "No active organizer profile was found for this account.";

                return RedirectToAction(
                    nameof(Register)
                );
            }

            return View(dashboard);
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            TempData["Error"] =
                "The organizer dashboard could not be loaded because the API is unavailable.";

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }

    /*
     * ==================================================
     * MANAGE ORGANIZER EVENTS
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> Events()
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Please log in before managing events.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        try
        {
            var events =
                await _organizerService
                    .GetEventsAsync(token);

            return View(events);
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            TempData["OrganizerMessage"] =
                "The events could not be loaded.";

            return RedirectToAction(
                nameof(Dashboard)
            );
        }
    }

    /*
     * ==================================================
     * CREATE EVENT
     * ==================================================
     */

    [HttpGet]
    public IActionResult CreateEvent()
    {
        if (!IsLoggedIn())
        {
            TempData["Error"] =
                "Please log in before creating an event.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        var model =
            new OrganizerEventFormViewModel
            {
                EventDate =
                    DateTime.Today.AddDays(1),

                EventTime =
                    new TimeSpan(9, 0, 0),

                TotalSeats = 1
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(
        OrganizerEventFormViewModel model)
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var created =
                await _organizerService
                    .CreateEventAsync(
                        token,
                        model
                    );

            if (!created)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The event could not be created. Confirm that your organizer profile is active and the event information is correct."
                );

                return View(model);
            }

            TempData["OrganizerMessage"] =
                "The event was created successfully.";

            return RedirectToAction(
                nameof(Events)
            );
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message
            );

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "The event could not be created because the API is unavailable."
            );

            return View(model);
        }
    }

    /*
     * ==================================================
     * EDIT EVENT
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> EditEvent(
        int id)
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Please log in before editing an event.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (id <= 0)
        {
            return BadRequest();
        }

        try
        {
            var eventItem =
                await _organizerService
                    .GetEventAsync(
                        token,
                        id
                    );

            if (eventItem is null)
            {
                return NotFound();
            }

            var model =
                new OrganizerEventFormViewModel
                {
                    Id = eventItem.Id,

                    Title =
                        eventItem.Title,

                    Description =
                        eventItem.Description,

                    EventDate =
                        eventItem.EventDate,

                    EventTime =
                        eventItem.EventTime,

                    Venue =
                        eventItem.Venue,

                    TicketPrice =
                        eventItem.TicketPrice,

                    TotalSeats =
                        eventItem.TotalSeats,

                    Category =
                        eventItem.Category
                };

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            TempData["OrganizerMessage"] =
                "The event could not be loaded.";

            return RedirectToAction(
                nameof(Events)
            );
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(
        int id,
        OrganizerEventFormViewModel model)
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (id <= 0)
        {
            return BadRequest();
        }

        model.Id = id;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var updated =
                await _organizerService
                    .UpdateEventAsync(
                        token,
                        id,
                        model
                    );

            if (!updated)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The event could not be updated. It may not exist, may not belong to you, or may already be cancelled."
                );

                return View(model);
            }

            TempData["OrganizerMessage"] =
                "The event was updated successfully.";

            return RedirectToAction(
                nameof(Events)
            );
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message
            );

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "The event could not be updated because the API is unavailable."
            );

            return View(model);
        }
    }

    /*
     * ==================================================
     * CANCEL EVENT
     * ==================================================
     */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelEvent(
        int id)
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (id <= 0)
        {
            return BadRequest();
        }

        try
        {
            var cancelled =
                await _organizerService
                    .CancelEventAsync(
                        token,
                        id
                    );

            TempData["OrganizerMessage"] =
                cancelled
                    ? "The event was cancelled successfully."
                    : "The event could not be cancelled.";

            return RedirectToAction(
                nameof(Events)
            );
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            TempData["OrganizerMessage"] =
                "The event could not be cancelled because the API is unavailable.";

            return RedirectToAction(
                nameof(Events)
            );
        }
    }

    /*
     * ==================================================
     * DELETE EVENT
     * ==================================================
     */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(
        int id)
    {
        var token = GetToken();

        if (token is null)
        {
            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (id <= 0)
        {
            return BadRequest();
        }

        try
        {
            var deleted =
                await _organizerService
                    .DeleteEventAsync(
                        token,
                        id
                    );

            TempData["OrganizerMessage"] =
                deleted
                    ? "The event was deleted successfully."
                    : "The event cannot be deleted because it has registrations. Cancel the event instead.";

            return RedirectToAction(
                nameof(Events)
            );
        }
        catch (UnauthorizedAccessException)
        {
            ClearLoginSession();

            TempData["Error"] =
                "Your login session has expired. Please log in again.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }
        catch (HttpRequestException)
        {
            TempData["OrganizerMessage"] =
                "The event could not be deleted because the API is unavailable.";

            return RedirectToAction(
                nameof(Events)
            );
        }
    }

    /*
     * ==================================================
     * PRIVATE HELPERS
     * ==================================================
     */

    private bool IsLoggedIn()
    {
        var userId =
            HttpContext.Session.GetInt32(
                "UserId"
            );

        var token =
            HttpContext.Session.GetString(
                "Token"
            );

        return userId.HasValue &&
               !string.IsNullOrWhiteSpace(token);
    }

    private string? GetToken()
    {
        var token =
            HttpContext.Session.GetString(
                "Token"
            );

        return string.IsNullOrWhiteSpace(token)
            ? null
            : token;
    }

    private void ClearLoginSession()
    {
        HttpContext.Session.Remove("Token");
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Remove("FirstName");
        HttpContext.Session.Remove("UserRole");
        HttpContext.Session.Remove("Role");
    }
}