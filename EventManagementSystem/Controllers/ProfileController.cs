using EventManagementSystem.DTO;
using EventManagementSystem.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(
        IProfileService profileService)
    {
        _profileService = profileService;
    }

    /*
     * ==================================================
     * USER PROFILE / DASHBOARD
     * URL: /Profile/Index
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        var profile =
            await _profileService.GetProfileAsync(
                userId.Value
            );

        if (profile is null)
        {
            TempData["Error"] =
                "Your profile could not be loaded.";

            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        return View(profile);
    }

    /*
     * ==================================================
     * EDIT PROFILE
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        var profile =
            await _profileService.GetProfileAsync(
                userId.Value
            );

        if (profile is null)
        {
            TempData["Error"] =
                "Your profile could not be loaded.";

            return RedirectToAction(
                nameof(Index)
            );
        }

        var dto =
            new UpdateProfileDto
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email
            };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result =
            await _profileService.UpdateProfileAsync(
                userId.Value,
                dto
            );

        if (!result)
        {
            TempData["Error"] =
                "Failed to update profile.";

            return View(dto);
        }

        /*
         * Keep the session name updated after
         * the profile information changes.
         */
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
        {
            HttpContext.Session.SetString(
                "FirstName",
                dto.FirstName.Trim()
            );
        }

        TempData["Success"] =
            "Profile updated successfully.";

        return RedirectToAction(
            nameof(Index)
        );
    }

    /*
     * ==================================================
     * BOOKED EVENTS
     * URL: /Profile/BookedEvents
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> BookedEvents()
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        var events =
            await _profileService.GetBookedEventsAsync(
                userId.Value
            );

        return View(
            events ?? new List<BookedEventDto>()
        );
    }

    /*
     * ==================================================
     * USER TICKETS
     * URL: /Profile/Tickets
     * ==================================================
     */

    [HttpGet]
    public async Task<IActionResult> Tickets()
    {
        var userId = GetCurrentUserId();

        if (!userId.HasValue)
        {
            return RedirectToAction(
                "Login",
                "Auth"
            );
        }

        var tickets =
            await _profileService.GetTicketEventsAsync(
                userId.Value
            );

        return View(
            tickets ?? new List<TicketEventDto>()
        );
    }

    /*
     * Gets the authenticated user's ID
     * from the frontend session.
     */
    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(
            "UserId"
        );
    }
}