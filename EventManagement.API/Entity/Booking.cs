namespace EventManagementAPI.Entity;
public class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int EventId { get; set; }

    public int Quantity { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; }
}