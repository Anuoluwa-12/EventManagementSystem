using EventManagementSystem.DTO;
using EventManagementSystem.Extensions;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers;

public class CartController : Controller
{
    public const string CartSessionKey = "LumieventTicketCart";

    [HttpGet]
    public IActionResult Index()
    {
        var cart = GetCart();

        return View(
            new CartPageViewModel
            {
                Items = cart
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(AddToCartDto request)
    {
        if (request.EventId <= 0)
        {
            TempData["Error"] = "The selected event is invalid.";

            return RedirectToAction("Index", "Event");
        }

        if (request.AvailableSeats <= 0)
        {
            TempData["Error"] = "This event is sold out.";

            return RedirectToAction("Details", "Event",
                new
                {
                    id = request.EventId
                }
            );
        }

        request.Quantity = Math.Clamp(request.Quantity, 1, Math.Min(request.AvailableSeats, 10));

        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(
                item => item.EventId == request.EventId
            );

        if (existingItem is null)
        {
            cart.Add(new CartItemDto
                {
                    EventId = request.EventId,
                    EventTitle = request.EventTitle,
                    EventImageUrl = request.EventImageUrl,
                    EventDate = request.EventDate,
                    UnitPrice = request.UnitPrice,
                    AvailableSeats = request.AvailableSeats,
                    Quantity = request.Quantity
                }
            );
        }
        else
        {
            existingItem.Quantity = Math.Min(existingItem.Quantity + request.Quantity, 
                Math.Min (request.AvailableSeats, 10)
                );
        }

        SaveCart(cart);

        TempData["Success"] = "Tickets added to your cart.";

        return RedirectToAction(nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int eventId, int quantity)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(cartItem => cartItem.EventId == eventId);

        if (item is null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (quantity <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = Math.Clamp(quantity, 1,
                    Math.Min(item.AvailableSeats, 10
                    )
                );
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int eventId)
    {
        var cart = GetCart();

        cart.RemoveAll(item => item.EventId == eventId
        );

        SaveCart(cart);

        TempData["Success"] = "The event was removed from your cart.";

        return RedirectToAction(nameof(Index)
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);

        return RedirectToAction(nameof(Index)
        );
    }

    private List<CartItemDto> GetCart()
    {
        return HttpContext.Session
            .GetObject<List<CartItemDto>>(CartSessionKey) ?? [];
    }

    private void SaveCart(List<CartItemDto> cart)
    {
        HttpContext.Session.SetObject(CartSessionKey, cart);
    }
}