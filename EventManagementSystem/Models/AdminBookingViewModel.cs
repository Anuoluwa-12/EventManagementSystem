namespace EventManagementSystem.Models
{
    public class AdminBookingViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string EventTitle { get; set; } =
            string.Empty;

        public int UserId { get; set; }

        public string AttendeeName { get; set; } =
            string.Empty;

        public string AttendeeEmail { get; set; } =
            string.Empty;

        public decimal AmountPaid { get; set; }

        public string BookingStatus { get; set; } =
            string.Empty;

        public string PaymentStatus { get; set; } =
            string.Empty;
    }

}
