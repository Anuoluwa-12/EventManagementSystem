using EventManagement.API.DTO;
using EventManagement.API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingController : ControllerBase
    {
        private readonly IUserService _userService;

        public OnboardingController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> ChooseAccountType(
            int userId,
            AccountTypeDto dto)
        {
            var result = await _userService
                .UpdateAccountTypeAsync(
                    userId,
                    dto.AccountType);

            if (!result)
                return NotFound();

            return Ok("Account type saved successfully");
        }
    }
}