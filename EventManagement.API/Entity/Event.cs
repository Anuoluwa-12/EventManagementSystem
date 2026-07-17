using EventManagement.API.Models;

namespace EventManagement.API.Entity;

public class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime EventDate { get; set; }

    public TimeSpan EventTime { get; set; }

    public string Venue { get; set; } = string.Empty;

    public decimal TicketPrice { get; set; }

    public int TotalSeats { get; set; }

    public int AvailableSeats { get; set; }

    public string? EventImageUrl { get; set; }

    public string Category { get; set; } = string.Empty;

    /*
     * This should point to the organizer_profiles table.
     * It is nullable so existing events without an organizer
     * do not immediately break.
     */
    public int? OrganizerId { get; set; }

    /*
     * This can remain temporarily, but the organizer profile
     * should eventually be the main source of the organizer name.
     */
    public string? OrganizerName { get; set; }

    /*
     * Recommended values:
     * Draft
     * Published
     * Cancelled
     * Completed
     */
    public string Status { get; set; } = "Published";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /*
     * Navigation property connecting the event
     * to its organizer profile.
     */
    public OrganizerProfile? Organizer { get; set; }

    /*
     * All registrations or bookings for this event.
     */
    public ICollection<EventRegistration> EventRegistrations { get; set; }
        = new List<EventRegistration>();
}