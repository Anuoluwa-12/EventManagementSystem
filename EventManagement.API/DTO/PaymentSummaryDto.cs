namespace EventManagement.API.DTO;

public class OrganizerPaymentSummaryDto
{
    public decimal TotalEarnings { get; set; }

    public decimal PendingAmount { get; set; }

    public int PaidBookings { get; set; }

    public int PendingBookings { get; set; }
}
