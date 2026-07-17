namespace EventManagement.API.DTO;

public class OrganizerEventDto
{
public int Id { get; set; }

public string Title { get; set; } = string.Empty;
public  string Description { get; set; }
public string Category { get; set; } = string.Empty;
public DateTime EventDate { get; set; }

public TimeSpan EventTime { get; set; }

public string Venue { get; set; } = string.Empty;

public decimal TicketPrice { get; set; }

public int TotalSeats { get; set; }

public int AvailableSeats { get; set; }

public int BookedSeats { get; set; }

public string Status { get; set; } = string.Empty;

public string? EventImageUrl { get; set; }
}
