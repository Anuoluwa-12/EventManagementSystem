namespace EventManagementSystem.DTO
{
    public class BookedEventDto
    {
        public int EventId { get; set; }

        public string EventTitle { get; set; }

        public DateTime EventDate { get; set; }

        public string Venue { get; set; }

        public string? EventImageUrl { get; set; }
    }
}
