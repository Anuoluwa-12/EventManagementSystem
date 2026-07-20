namespace EventManagement.API.DTO
{
    public class AdminPaymentDto
    {
        public int BookingId { get; set; }

        public string EventTitle { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string BookingStatus { get; set; } = string.Empty;
    }


}
