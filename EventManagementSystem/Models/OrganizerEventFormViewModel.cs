using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models;
public class OrganizerEventFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public TimeSpan EventTime { get; set; }

    [Required]
    public string Venue { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TicketPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalSeats { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    public IFormFile? EventImage { get; set; }
}
