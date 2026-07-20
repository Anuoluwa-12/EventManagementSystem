namespace EventManagement.API.DTO
{
    public class AdminEventDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string OrganizerName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        public string Venue { get; set; } = string.Empty;

        public decimal TicketPrice { get; set; }

        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        public string Status { get; set; } = string.Empty;
    }

}
