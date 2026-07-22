namespace EventManagementSystem.DTO
{
    public class EventDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime EventDate { get; set; }

        public string Venue { get; set; }

        public decimal TicketPrice { get; set; }

        public string Category { get; set; }
        public  int AvailableSeats { get; set; }

        public string EventImageUrl { get; set; }

        public string OrganizerName { get; set; }
    }
}


