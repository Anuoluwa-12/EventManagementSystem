using Microsoft.AspNetCore.Mvc;
using EventManagement.MVC.Models;
using EventManagementSystem.Interface;
using EventManagementSystem.DTO;

namespace EventManagement.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result)
            {
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Registration failed. Please try again.";

            return View(dto);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
            {
                TempData["Error"] = "Invalid email or password.";
                return View(dto);
            }

            // Store information in Session
            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("FirstName", result.FirstName);
            HttpContext.Session.SetString("Role", result.Role);

            TempData["Success"] = "Login successful!";

            return RedirectToAction("ChooseType", "Onboarding");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult CorporateOnboarding()
        {
            return View();
        }
    }
}