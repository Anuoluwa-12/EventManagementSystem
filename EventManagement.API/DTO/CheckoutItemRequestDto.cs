using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.DTO;

public class CheckoutItemRequestDto
{
    [Range(1, int.MaxValue)]
    public int EventId { get; set; }

    [Range(1, 10)]
    public int Quantity { get; set; }
}




