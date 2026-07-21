using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DTO;
 public class AddToCartDto
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string? EventImageUrl { get; set; }
    public DateTime EventDate { get; set; }
    public decimal UnitPrice { get; set; }
    public int AvailableSeats { get; set; }
    [Range(1, 10)]
    public int Quantity { get; set; } = 1;
}










