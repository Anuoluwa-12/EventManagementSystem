using EventManagement.API.Data;
using EventManagement.API.DTO;
using EventManagement.API.DTO.Payment;
using EventManagement.API.Entity;
using EventManagement.API.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace EventManagement.API.Service;

public class CheckoutService : ICheckoutService
{
    private const int MaximumTicketsPerEvent = 10;

    private readonly ApplicationDbContext _context;
    private readonly IPaystackService _paystackService;

    public CheckoutService(ApplicationDbContext context, IPaystackService paystackService)
    {
        _context = context;
        _paystackService = paystackService;
    }

    public async Task<InitializeCheckoutResponseDto>InitializeAsync(InitializeCheckoutRequestDto request)
    {
        if (request.Items.Count == 0)
        {
            return Failure(
                "Your cart is empty."
            );
        }

        var userExists =
            await _context.Users
                .AsNoTracking()
                .AnyAsync(
                    user => user.Id == request.UserId
                );

        if (!userExists)
        {
            return Failure(
                "The selected user does not exist."
            );
        }

        /*
         * Combine duplicate events in the cart.
         */
        var groupedItems =
            request.Items
                .GroupBy(item => item.EventId)
                .Select(group =>
                    new CheckoutItemRequestDto
                    {
                        EventId = group.Key,
                        Quantity = group.Sum(
                            item => item.Quantity
                        )
                    }
                )
                .ToList();

        if (groupedItems.Any(
            item =>
                item.Quantity < 1
                ||
                item.Quantity >
                    MaximumTicketsPerEvent))
        {
            return Failure(
                $"You can purchase a maximum of " +
                $"{MaximumTicketsPerEvent} tickets " +
                "for one event."
            );
        }

        var eventIds =
            groupedItems
                .Select(item => item.EventId)
                .ToList();

        var events =
            await _context.Events
                .Where(
                    eventItem =>
                        eventIds.Contains(eventItem.Id)
                )
                .ToListAsync();

        if (events.Count != eventIds.Count)
        {
            return Failure(
                "One or more events could not be found."
            );
        }

        decimal checkoutTotal = 0m;

        foreach (var cartItem in groupedItems)
        {
            var eventItem =
                events.Single(
                    eventRecord =>
                        eventRecord.Id ==
                        cartItem.EventId
                );

            if (eventItem.AvailableSeats <
                cartItem.Quantity)
            {
                return Failure(
                    $"Only {eventItem.AvailableSeats} " +
                    $"ticket(s) remain for " +
                    $"{eventItem.Title}."
                );
            }

            if (string.Equals(
                eventItem.Status,
                "Cancelled",
                StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    eventItem.Status,
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Failure(
                    $"{eventItem.Title} is not " +
                    "available for booking."
                );
            }

            checkoutTotal +=
                eventItem.TicketPrice *
                cartItem.Quantity;
        }

        if (checkoutTotal <= 0)
        {
            return Failure(
                "The checkout total is invalid."
            );
        }

        var reference =
            GeneratePaymentReference();

        var registrations =
            groupedItems.Select(cartItem =>
            {
                var eventItem =
                    events.Single(
                        eventRecord =>
                            eventRecord.Id ==
                            cartItem.EventId
                    );

                var lineTotal =
                    eventItem.TicketPrice *
                    cartItem.Quantity;

                return new EventRegistration
                {
                    EventId = eventItem.Id,
                    UserId = request.UserId,
                    Quantity = cartItem.Quantity,
                    TotalAmount = lineTotal,
                    AmountPaid = 0m,
                    PaymentReference = reference,
                    BookingStatus = "Pending",
                    PaymentStatus = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
            }).ToList();

        _context.EventRegistrations
            .AddRange(registrations);

        await _context.SaveChangesAsync();

        var amountInKobo =
            ConvertToKobo(checkoutTotal);

        var paystackResponse =
            await _paystackService
                .InitializeTransactionAsync(
                    new PaystackInitializeRequest
                    {
                        Email = request.Email.Trim(),
                        Amount =
                            amountInKobo.ToString(
                                CultureInfo.InvariantCulture
                            ),
                        Reference = reference,
                        CallbackUrl =
                            request.CallbackUrl,
                        Currency = "NGN",
                        Metadata =
                            JsonSerializer.Serialize(
                                new
                                {
                                    userId =
                                        request.UserId,

                                    paymentReference =
                                        reference,

                                    itemCount =
                                        groupedItems.Count
                                }
                            )
                    }
                );

        if (paystackResponse is null
            ||
            !paystackResponse.Status
            ||
            paystackResponse.Data is null
            ||
            string.IsNullOrWhiteSpace(
                paystackResponse.Data
                    .AuthorizationUrl
            ))
        {
            _context.EventRegistrations
                .RemoveRange(registrations);

            await _context.SaveChangesAsync();

            return Failure(
                paystackResponse?.Message
                ??
                "Payment could not be initialized."
            );
        }

        return new InitializeCheckoutResponseDto
        {
            Success = true,
            Message =
                "Payment initialized successfully.",
            AuthorizationUrl =
                paystackResponse.Data
                    .AuthorizationUrl,
            Reference = reference
        };
    }

    public async Task<VerifyPaymentResponseDto>VerifyAsync(string reference, int userId)
    {
        var registrations =
            await _context.EventRegistrations
                .Where(
                    booking =>
                        booking.PaymentReference ==
                            reference
                        &&
                        booking.UserId == userId
                )
                .ToListAsync();

        if (registrations.Count == 0)
        {
            return VerificationFailure(
                reference,
                "No booking was found for this payment."
            );
        }

        /*
         * Makes repeated callbacks safe.
         */
        if (registrations.All(
            booking =>
                booking.PaymentStatus == "Paid"))
        {
            return await BuildSuccessfulResponseAsync(
                reference,
                registrations
            );
        }

        var paystackResponse =
            await _paystackService
                .VerifyTransactionAsync(reference);

        if (paystackResponse is null
            ||
            !paystackResponse.Status
            ||
            paystackResponse.Data is null
            ||
            !string.Equals(
                paystackResponse.Data.Status,
                "success",
                StringComparison.OrdinalIgnoreCase))
        {
            return VerificationFailure(
                reference,
                paystackResponse?.Message
                ??
                "Payment was not successful."
            );
        }

        var expectedTotal =
            registrations.Sum(
                booking => booking.TotalAmount
            );

        var expectedKobo =
            ConvertToKobo(expectedTotal);

        if (paystackResponse.Data.Amount !=
            expectedKobo)
        {
            return VerificationFailure(
                reference,
                "The verified payment amount " +
                "does not match the booking total."
            );
        }

        if (!string.Equals(
            paystackResponse.Data.Currency,
            "NGN",
            StringComparison.OrdinalIgnoreCase))
        {
            return VerificationFailure(
                reference,
                "The payment currency is invalid."
            );
        }

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync();

        try
        {
            foreach (var booking in registrations)
            {
                /*
                 * Conditional SQL update prevents the
                 * available seat value from going below zero.
                 */
                var affectedRows =
                    await _context.Database
                        .ExecuteSqlInterpolatedAsync(
                            $"""
                            UPDATE dbo.Events
                            SET AvailableSeats =
                                AvailableSeats -
                                {booking.Quantity}
                            WHERE Id = {booking.EventId}
                              AND AvailableSeats >=
                                  {booking.Quantity}
                            """
                        );

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException(
                        "One of the selected events " +
                        "no longer has enough seats."
                    );
                }

                booking.AmountPaid =
                    booking.TotalAmount;

                booking.PaymentStatus = "Paid";
                booking.BookingStatus = "Confirmed";

                for (
                    var ticketIndex = 0;
                    ticketIndex < booking.Quantity;
                    ticketIndex++)
                {
                    _context.Tickets.Add(
                        new Ticket
                        {
                            BookingId = booking.Id,
                            TicketNumber =
                                GenerateTicketNumber(),
                            QRCodeUrl = null,
                            Status = "Valid",
                            CreatedAt =
                                DateTime.UtcNow
                        }
                    );
                }
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            foreach (var ticketEntry in
                _context.ChangeTracker
                    .Entries<Ticket>()
                    .Where(
                        entry =>
                            entry.State ==
                            EntityState.Added
                    ))
            {
                ticketEntry.State =
                    EntityState.Detached;
            }

            foreach (var booking in registrations)
            {
                booking.AmountPaid =
                    booking.TotalAmount;

                booking.PaymentStatus =
                    "ReviewRequired";

                booking.BookingStatus =
                    "PaymentReceivedAwaitingReview";
            }

            await _context.SaveChangesAsync();

            return VerificationFailure(
                reference,
                exception.Message
            );
        }

        return await BuildSuccessfulResponseAsync(
            reference,
            registrations
        );
    }

    private async Task<VerifyPaymentResponseDto>BuildSuccessfulResponseAsync(string reference, List<EventRegistration> registrations)
    {
        var bookingIds =
            registrations
                .Select(booking => booking.Id)
                .ToList();

        var tickets =
            await (
                from ticket in _context.Tickets
                join booking in
                    _context.EventRegistrations
                    on ticket.BookingId
                    equals booking.Id
                join eventItem in _context.Events
                    on booking.EventId
                    equals eventItem.Id
                where bookingIds.Contains(
                    booking.Id
                )
                select new PurchasedTicketDto
                {
                    TicketId = ticket.Id,
                    EventId = eventItem.Id,
                    EventTitle =
                        eventItem.Title,
                    TicketNumber =
                        ticket.TicketNumber,
                    Status = ticket.Status
                }
            )
            .AsNoTracking()
            .ToListAsync();

        return new VerifyPaymentResponseDto
        {
            Success = true,
            Message =
                "Payment verified and tickets " +
                "generated successfully.",
            Reference = reference,
            AmountPaid =
                registrations.Sum(
                    booking => booking.TotalAmount
                ),
            Tickets = tickets
        };
    }

    private static InitializeCheckoutResponseDto
        Failure(string message)
    {
        return new InitializeCheckoutResponseDto
        {
            Success = false,
            Message = message
        };
    }

    private static VerifyPaymentResponseDto
        VerificationFailure( string reference, string message)
    {
        return new VerifyPaymentResponseDto
        {
            Success = false,
            Reference = reference,
            Message = message
        };
    }

    private static long ConvertToKobo(decimal amount)
    {
        return decimal.ToInt64(
            decimal.Round(
                amount * 100m,
                0,
                MidpointRounding.AwayFromZero
            )
        );
    }

    private static string GeneratePaymentReference()
    {
        return
            $"LUM-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
            Guid.NewGuid()
                .ToString("N")[..10]
                .ToUpperInvariant();
    }

    private static string GenerateTicketNumber()
    {
        return
            $"LUM-{DateTime.UtcNow.Year}-" +
            Guid.NewGuid()
                .ToString("N")[..10]
                .ToUpperInvariant();
    }
}