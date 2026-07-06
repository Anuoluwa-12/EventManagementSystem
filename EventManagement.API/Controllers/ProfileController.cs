using EventManagement.API.Interface;
using Microsoft.AspNetCore.Mvc;
using EventManagement.API.DTO;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile =
                await _userService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(
     int userId,
     [FromForm] UpdateProfileDto dto)
        {
            var result = await _userService.UpdateProfileAsync(userId, dto);

            if (!result)
                return NotFound();

            return Ok("Profile updated successfully");
        }

        [HttpGet("{userId}/booked-events")]
        public async Task<IActionResult> GetBookedEvents(int userId)
        {
            var events = await _userService.GetBookedEventsAsync(userId);

            return Ok(events);
        }

        [HttpGet("{userId}/tickets")]
        public async Task<IActionResult> GetTickets(int userId)
        {
            var tickets = await _userService.GetTicketsAsync(userId);

            return Ok(tickets);
        }
    }
}