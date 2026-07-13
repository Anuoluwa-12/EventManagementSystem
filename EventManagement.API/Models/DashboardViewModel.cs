namespace EventManagement.API.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalBookings { get; set; }

        public int TotalOrganizers { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TotalEvents { get; set; }

        public int UpcomingEvents { get; set; }

        public int PendingBookings { get; set; }

        public int ConfirmedBookings { get; set; }
    }
}
