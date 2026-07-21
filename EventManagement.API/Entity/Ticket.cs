namespace EventManagement.API.Entity;
public class Ticket
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string? QRCodeUrl { get; set; }
    public string Status { get; set; } = "Valid";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }
}