namespace EventManagement.API.DTO
{
    public class AdminReportDto
    {
        public int TotalUsers { get; set; }

        public int TotalOrganizers { get; set; }

        public int ActiveOrganizers { get; set; }

        public int TotalEvents { get; set; }

        public int UpcomingEvents { get; set; }

        public int TotalBookings { get; set; }

        public int PaidBookings { get; set; }

        public int PendingBookings { get; set; }

        public int CancelledBookings { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
