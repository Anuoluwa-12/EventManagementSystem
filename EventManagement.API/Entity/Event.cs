using EventManagement.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.API.Entity;

public class Event
{
    [Key]
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

    public int? OrganizerId { get; set; }

    public string? OrganizerName { get; set; }

    public string Status { get; set; } = "Published";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public OrganizerProfile? Organizer { get; set; }

    public ICollection<EventRegistration> EventRegistrations { get; set; }
        = new List<EventRegistration>();
}