namespace EventManagement.API.DTO;

public class OrganizerAttendeeDto
{
    public int RegistrationId { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal AmountPaid { get; set; }

    public string BookingStatus { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; }
}