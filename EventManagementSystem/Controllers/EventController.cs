using EventManagementSystem.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.MVC.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

    public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index(string? search, string? category, string? location, DateTime? date, decimal? maxPrice)
        {
            var events = await _eventService.GetAllEventsAsync(
                search,
                category,
                location,
                date,
                maxPrice);

            return View(events);
        }
        [HttpPost]
        public async Task<IActionResult> BookEvent(int eventId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            await _eventService.BookEventAsync(userId.Value, eventId);

            return RedirectToAction("BookedEvents", "Profile");
        }
    }
}
