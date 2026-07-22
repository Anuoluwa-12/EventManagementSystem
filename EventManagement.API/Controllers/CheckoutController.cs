using EventManagement.API.DTO;
using EventManagement.API.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize(InitializeCheckoutRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkoutService.InitializeAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("verify/{reference}")]
    public async Task<IActionResult> Verify(string reference, [FromQuery] int userId)
    {
        if (string.IsNullOrWhiteSpace(reference) || userId <= 0)
        {
            return BadRequest(new VerifyPaymentResponseDto
                {
                    Success = false,
                    Reference = reference ?? "",
                    Message = "A payment reference and " + "user ID are required."
                }
            );
        }

        var result = await _checkoutService.VerifyAsync(reference, userId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}