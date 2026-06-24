using EventManagement.API.DTO;
using EventManagement.API.Interface;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    //  REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _userService.RegisterAsync(dto);

        if (result == "User already exists")
            return BadRequest(result);

        return Ok(result);
    }

    //  LOGIN 
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);

        if (result == null)
            return Unauthorized("Invalid credentials");

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var result = await _userService.ForgotPasswordAsync(dto);


        if (!result)
            return BadRequest("User not found");

        return Ok("Password reset link sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await _userService.ResetPasswordAsync(dto);

        if (result == null)
            return BadRequest("Invalid or expired token");

        return Ok("Password reset successful");
    }

    [HttpPost("corporate-onboarding")]
    public async Task<IActionResult> CorporateOnboarding(
    CorporateOnboardingDto dto)
    {
        var result =
            await _userService.CorporateOnboardingAsync(dto);

        return Ok(result);
    }
}


