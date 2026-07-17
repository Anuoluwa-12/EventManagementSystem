namespace EventManagementSystem.Models;
public class OrganizerDashboardViewModel
{
    public int TotalEvents { get; set; }

    public int ActiveEvents { get; set; }

    public decimal TotalEarnings { get; set; }

    public int PendingBookings { get; set; }

    public int PaidBookings { get; set; }
}
