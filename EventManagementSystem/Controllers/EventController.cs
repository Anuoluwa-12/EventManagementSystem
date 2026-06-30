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

        public async Task<IActionResult> Index()
        {
            var events =
                await _eventService.GetAllEventsAsync();

            return View(events);
        }
    }
}
