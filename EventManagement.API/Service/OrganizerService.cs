using EventManagement.API.Data;
using EventManagement.API.DTO;
using EventManagement.API.Entity;
using EventManagement.API.Interface;
using EventManagement.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.API.Service;

public class OrganizerService : IOrganizerService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] AllowedImageExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private const long MaximumImageSize =
        5 * 1024 * 1024;

    public OrganizerService( ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /*
     * ==================================================
     * REGISTER AS AN ORGANIZER
     * ==================================================
     */

    public async Task<bool> RegisterAsync( int userId, RegisterOrganizerRequestDto request)
    {
        var user =
            await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new InvalidOperationException(
                $"The authenticated user with ID {userId} was not found."
            );
        }

        var existingOrganizer =
            await _context.OrganizerProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

        if (existingOrganizer is not null)
        {
            if (string.Equals(
                existingOrganizer.Status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You already have an active organizer profile."
                );
            }

            existingOrganizer.DisplayName =
                request.DisplayName.Trim();

            existingOrganizer.Phone =
                request.Phone?.Trim();

            existingOrganizer.Bio =
                request.Bio?.Trim();

            existingOrganizer.Status =
                "Active";

            await _context.SaveChangesAsync();

            return true;
        }

        var organizer =
            new OrganizerProfile
            {
                UserId = userId,

                DisplayName =
                    request.DisplayName.Trim(),

                Phone =
                    request.Phone?.Trim(),

                Bio =
                    request.Bio?.Trim(),

                Status = "Active",

                CreatedAt = DateTime.UtcNow
            };

        _context.OrganizerProfiles.Add(organizer);

        await _context.SaveChangesAsync();

        return true;
    }

    /*
     * ==================================================
     * ORGANIZER DASHBOARD
     * ==================================================
     */

    public async Task<OrganizerDashboardDto?>
        GetDashboardAsync(int userId)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return null;
        }

        var eventQuery =
            _context.Events
                .AsNoTracking()
                .Where(x =>
                    x.OrganizerId == organizer.Id
                );

        var organizerEventIds =
            eventQuery.Select(x => x.Id);

        var registrationQuery =
            _context.EventRegistrations
                .AsNoTracking()
                .Where(x =>
                    organizerEventIds.Contains(
                        x.EventId
                    )
                );

        var totalEvents =
            await eventQuery.CountAsync();

        var activeEvents =
            await eventQuery.CountAsync(x =>
                x.Status == "Published"
            );

        var totalEarnings =
            await registrationQuery
                .Where(x =>
                    x.PaymentStatus == "Paid" &&
                    x.BookingStatus != "Cancelled"
                )
                .SumAsync(x =>
                    (decimal?)x.AmountPaid
                )
            ?? 0m;

        var pendingBookings =
            await registrationQuery.CountAsync(x =>
                x.PaymentStatus == "Pending" &&
                x.BookingStatus != "Cancelled"
            );

        var paidBookings =
            await registrationQuery.CountAsync(x =>
                x.PaymentStatus == "Paid" &&
                x.BookingStatus != "Cancelled"
            );

        return new OrganizerDashboardDto
        {
            TotalEvents = totalEvents,

            ActiveEvents = activeEvents,

            TotalEarnings = totalEarnings,

            PendingBookings = pendingBookings,

            PaidBookings = paidBookings
        };
    }

    /*
     * ==================================================
     * GET ALL ORGANIZER EVENTS
     * ==================================================
     */

    public async Task<List<OrganizerEventDto>>
        GetEventsAsync(int userId)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return new List<OrganizerEventDto>();
        }

        return await _context.Events
            .AsNoTracking()
            .Where(x =>
                x.OrganizerId == organizer.Id
            )
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrganizerEventDto
            {
                Id = x.Id,

                Title = x.Title,

                Description = x.Description,

                EventDate = x.EventDate,

                EventTime = x.EventTime,

                Venue = x.Venue,

                TicketPrice = x.TicketPrice,

                TotalSeats = x.TotalSeats,

                AvailableSeats = x.AvailableSeats,

                BookedSeats =
                    x.TotalSeats -
                    x.AvailableSeats,

                Category = x.Category,

                Status = x.Status,

                EventImageUrl =
                    x.EventImageUrl
            })
            .ToListAsync();
    }

    /*
     * ==================================================
     * GET ONE ORGANIZER EVENT
     * ==================================================
     */

    public async Task<OrganizerEventDto?>
        GetEventAsync(
            int userId,
            int eventId)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return null;
        }

        return await _context.Events
            .AsNoTracking()
            .Where(x =>
                x.Id == eventId &&
                x.OrganizerId == organizer.Id
            )
            .Select(x => new OrganizerEventDto
            {
                Id = x.Id,

                Title = x.Title,

                Description = x.Description,

                EventDate = x.EventDate,

                EventTime = x.EventTime,

                Venue = x.Venue,

                TicketPrice = x.TicketPrice,

                TotalSeats = x.TotalSeats,

                AvailableSeats = x.AvailableSeats,

                BookedSeats =
                    x.TotalSeats -
                    x.AvailableSeats,

                Category = x.Category,

                Status = x.Status,

                EventImageUrl =
                    x.EventImageUrl
            })
            .FirstOrDefaultAsync();
    }

    /*
     * ==================================================
     * CREATE EVENT
     * ==================================================
     */

    public async Task<int?> CreateEventAsync(int userId, OrganizerEventRequestDto request)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return null;
        }

        ValidateEventDateAndTime(
            request.EventDate,
            request.EventTime
        );

        var eventImageUrl =
            await SaveEventImageAsync(
                request.EventImage
            );

        var eventItem =
            new Event
            {
                Title =
                    request.Title.Trim(),

                Description =
                    request.Description.Trim(),

                EventDate =
                    request.EventDate.Date,

                EventTime =
                    request.EventTime,

                Venue =
                    request.Venue.Trim(),

                TicketPrice =
                    request.TicketPrice,

                TotalSeats =
                    request.TotalSeats,

                // A newly created event has no bookings.
                AvailableSeats =
                    request.TotalSeats,

                Category =
                    request.Category.Trim(),

                EventImageUrl =
                    eventImageUrl,

                // This is organizer_profiles.Id,
                // not the user's ID.
                OrganizerId =
                    organizer.Id,

                OrganizerName =
                    organizer.DisplayName,

                Status =
                    "Published",

                CreatedAt =
                    DateTime.UtcNow
            };

        _context.Events.Add(eventItem);

        await _context.SaveChangesAsync();

        return eventItem.Id;
    }

    /*
     * ==================================================
     * UPDATE EVENT
     * ==================================================
     */

    public async Task<bool> UpdateEventAsync(int userId, int eventId, OrganizerEventRequestDto request)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return false;
        }

        var eventItem =
            await _context.Events
                .FirstOrDefaultAsync(x =>
                    x.Id == eventId &&
                    x.OrganizerId == organizer.Id
                );

        if (eventItem is null)
        {
            return false;
        }

        if (string.Equals(
            eventItem.Status,
            "Cancelled",
            StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        ValidateEventDateAndTime(
            request.EventDate,
            request.EventTime
        );

        var bookedSeats =
            Math.Max(
                0,
                eventItem.TotalSeats -
                eventItem.AvailableSeats
            );

        /*
         * The organizer cannot reduce the total number
         * of seats below the number already booked.
         */
        if (request.TotalSeats < bookedSeats)
        {
            throw new InvalidOperationException(
                $"This event already has {bookedSeats} booked seats. Total seats cannot be lower than that."
            );
        }

        eventItem.Title =
            request.Title.Trim();

        eventItem.Description =
            request.Description.Trim();

        eventItem.EventDate =
            request.EventDate.Date;

        eventItem.EventTime =
            request.EventTime;

        eventItem.Venue =
            request.Venue.Trim();

        eventItem.TicketPrice =
            request.TicketPrice;

        eventItem.TotalSeats =
            request.TotalSeats;

        /*
         * Preserve the seats already booked while
         * recalculating the available seats.
         */
        eventItem.AvailableSeats =
            request.TotalSeats -
            bookedSeats;

        eventItem.Category =
            request.Category.Trim();

        eventItem.OrganizerName =
            organizer.DisplayName;

        eventItem.UpdatedAt =
            DateTime.UtcNow;

        /*
         * Only replace the event image when a new
         * image was uploaded.
         */
        if (request.EventImage is not null &&
            request.EventImage.Length > 0)
        {
            eventItem.EventImageUrl =
                await SaveEventImageAsync(
                    request.EventImage
                );
        }

        await _context.SaveChangesAsync();

        return true;
    }

    /*
     * ==================================================
     * CANCEL EVENT
     * ==================================================
     */

    public async Task<bool> CancelEventAsync(int userId, int eventId)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return false;
        }

        var eventItem =
            await _context.Events
                .FirstOrDefaultAsync(x =>
                    x.Id == eventId &&
                    x.OrganizerId == organizer.Id
                );

        if (eventItem is null)
        {
            return false;
        }

        eventItem.Status =
            "Cancelled";

        eventItem.UpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    /*
     * ==================================================
     * DELETE EVENT
     * ==================================================
     */

    public async Task<bool> DeleteEventAsync(
        int userId,
        int eventId)
    {
        var organizer =
            await GetActiveOrganizerAsync(userId);

        if (organizer is null)
        {
            return false;
        }

        var eventItem =
            await _context.Events
                .FirstOrDefaultAsync(x =>
                    x.Id == eventId &&
                    x.OrganizerId == organizer.Id
                );

        if (eventItem is null)
        {
            return false;
        }

        var hasRegistrations =
            await _context.EventRegistrations
                .AnyAsync(x =>
                    x.EventId == eventId
                );

        /*
         * Events with bookings must be cancelled
         * instead of permanently deleted.
         */
        if (hasRegistrations)
        {
            return false;
        }

        _context.Events.Remove(eventItem);

        await _context.SaveChangesAsync();

        return true;
    }

    /*
     * ==================================================
     * PRIVATE HELPERS
     * ==================================================
     */

    private async Task<OrganizerProfile?>
        GetActiveOrganizerAsync(int userId)
    {
        return await _context.OrganizerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Status == "Active"
            );
    }

    private static void ValidateEventDateAndTime(
        DateTime eventDate,
        TimeSpan eventTime)
    {
        var fullEventDateTime =
            eventDate.Date.Add(eventTime);

        if (fullEventDateTime <= DateTime.Now)
        {
            throw new InvalidOperationException(
                "The event date and time must be in the future."
            );
        }
    }

    private async Task<string?>
        SaveEventImageAsync(IFormFile? image)
    {
        if (image is null ||
            image.Length == 0)
        {
            return null;
        }

        if (image.Length > MaximumImageSize)
        {
            throw new InvalidOperationException(
                "The event image must not be larger than 5 MB."
            );
        }

        var extension =
            Path.GetExtension(image.FileName)
                .ToLowerInvariant();

        if (!AllowedImageExtensions.Contains(
            extension))
        {
            throw new InvalidOperationException(
                "Only JPG, JPEG, PNG and WEBP images are allowed."
            );
        }

        var webRootPath =
            _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(
            webRootPath))
        {
            webRootPath =
                Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot"
                );
        }

        var eventUploadFolder =
            Path.Combine(
                webRootPath,
                "uploads",
                "events"
            );

        Directory.CreateDirectory(
            eventUploadFolder
        );

        var generatedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var completeFilePath =
            Path.Combine(
                eventUploadFolder,
                generatedFileName
            );

        await using var fileStream =
            new FileStream(
                completeFilePath,
                FileMode.Create
            );

        await image.CopyToAsync(
            fileStream
        );

        return
            $"/uploads/events/{generatedFileName}";
    }
}