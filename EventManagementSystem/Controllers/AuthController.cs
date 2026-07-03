using Microsoft.AspNetCore.Mvc;
using EventManagement.MVC.Models;
using EventManagementSystem.Interface;
using EventManagementSystem.DTO;
using System.Net.Http.Json;

namespace EventManagement.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly HttpClient _httpClient;

        public AuthController(IAuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;
        }

        // REGISTER 
        public IActionResult Register()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ChooseType()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChooseType(string accountType)
        {
            if (accountType == "Individual")
            {
                return RedirectToAction("Register");
            }

            if (accountType == "Corporate")
            {
                return RedirectToAction("CorporateOnboarding");
            }

            TempData["Error"] = "Please select an account type.";
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

        // LOGIN 
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

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("FirstName", result.FirstName);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetInt32("UserId", result.Id);

            TempData["Success"] = "Login successful!";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CorporateOnboarding()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CorporateOnboarding(CorporateOnboardingDto model)
        {
            //  Validate before sending
            if (string.IsNullOrWhiteSpace(model.CompanyName) ||
                string.IsNullOrWhiteSpace(model.CompanyEmail))
            {
                ModelState.AddModelError("", "Company Name and Email are required");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            //  Send clean request to API
            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7053/api/auth/corporate-onboarding",
                model
            );

            //  Handle API failure
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Corporate onboarding failed. Please try again.";
                return View(model);
            }

            TempData["Success"] = "Corporate account created successfully!";
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}