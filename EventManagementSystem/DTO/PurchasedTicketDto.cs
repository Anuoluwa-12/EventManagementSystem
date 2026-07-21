namespace EventManagementSystem.DTO
{
    public class PurchasedTicketDto
    {
        public int TicketId { get; set; }

        public int EventId { get; set; }

        public string EventTitle { get; set; } =
            string.Empty;

        public string TicketNumber { get; set; } =
            string.Empty;

        public string Status { get; set; } =
            string.Empty;
    }

}
