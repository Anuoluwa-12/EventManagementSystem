namespace EventManagementSystem.Models;

public sealed class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }

    public int TotalBookings { get; set; }

    public int TotalOrganizers { get; set; }

    public decimal TotalRevenue { get; set; }

    public int TotalEvents { get; set; }
}