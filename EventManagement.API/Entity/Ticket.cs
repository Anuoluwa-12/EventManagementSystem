namespace EventManagement.API.Entity
{
    public class Ticket
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public string TicketNumber { get; set; }

        public string QRCodeUrl { get; set; }

        public string Status { get; set; }
    }
}