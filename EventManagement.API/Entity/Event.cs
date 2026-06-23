namespace EventManagement.API.Entity;
public class Event
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime EventDate { get; set; }

    public TimeSpan EventTime { get; set; }

    public string Venue { get; set; }

    public decimal TicketPrice { get; set; }

    public int TotalSeats { get; set; }

    public int AvailableSeats { get; set; }

    public string BannerImage { get; set; }

    public int CategoryId { get; set; }

    public string OrganizerId { get; set; }
}