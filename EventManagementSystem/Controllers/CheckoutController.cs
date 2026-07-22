using EventManagementSystem.DTO;
using EventManagementSystem.Extensions;
using EventManagementSystem.Interface;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class CheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var cart = GetCart();

        if (cart.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel
            {
                Email = HttpContext.Session.GetString("Email") ?? string.Empty,
                Items = cart
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(CheckoutViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var cart = GetCart();

        model.Items = cart;

        if (cart.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Your cart is empty.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var callbackUrl = Url.Action(nameof(Callback), "Checkout",
                values: null,
                protocol: Request.Scheme
            );

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            ModelState.AddModelError(string.Empty, "The payment callback URL " + "could not be generated."
            );

            return View("Index", model);
        }

        var result = await _checkoutService.InitializeAsync(new InitializeCheckoutRequestDto
                    {
                        UserId = userId.Value,
                        Email = model.Email,
                        CallbackUrl = callbackUrl,
                        Items = cart.Select(
                                item => new CheckoutItemRequestDto
                                    {
                                     EventId = item.EventId,
                                     Quantity = item.Quantity
                                    }
                            )
                            .ToList()
                    }
                );

        if (!result.Success || string.IsNullOrWhiteSpace(result.AuthorizationUrl))
        {
            ModelState.AddModelError(string.Empty, result.Message);

            return View("Index", model);
        }

        return Redirect(result.AuthorizationUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Callback(string? reference, string? trxref)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var paymentReference = reference ?? trxref;

        if (string.IsNullOrWhiteSpace(paymentReference))
        {
            return View("Failed", "The payment reference is missing.");
        }

        var result = await _checkoutService.VerifyAsync(paymentReference, userId.Value);

        if (!result.Success)
        {
            return View("Failed", result.Message);
        }

        HttpContext.Session.Remove(CartController.CartSessionKey);

        return View("Success", result);
    }

    private List<CartItemDto> GetCart()
    {
        return HttpContext.Session.GetObject<List<CartItemDto>>(CartController.CartSessionKey) ?? [];
    }
}