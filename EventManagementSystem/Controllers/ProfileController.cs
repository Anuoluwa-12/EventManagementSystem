using EventManagementSystem.DTO;
using EventManagementSystem.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var profile =
              await _profileService.GetProfileAsync(userId.Value);

            return View(profile);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var profile =
              await _profileService.GetProfileAsync(userId.Value);

            var dto = new UpdateProfileDto
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email
            };

            return View(dto);
        }

        // POST: Profile/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProfileDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _profileService.UpdateProfileAsync(userId.Value, dto);

            if (result)
            {
                TempData["Success"] = "Profile updated successfully";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to update profile";
            return View(dto);
        }
    }
}