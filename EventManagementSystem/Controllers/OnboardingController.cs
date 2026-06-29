using Microsoft.AspNetCore.Mvc;

namespace EventManagement.MVC.Controllers
{
    public class OnboardingController : Controller
    {
        public IActionResult ChooseType()
        {
            return View();
        }

    public IActionResult Individual()
        {
            return View();
        }

        public IActionResult Corporate()
        {
            return View();
        }
    }

}
