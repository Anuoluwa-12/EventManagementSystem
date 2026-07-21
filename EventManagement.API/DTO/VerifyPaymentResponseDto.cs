namespace EventManagement.API.DTO
{
    public class VerifyPaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public List<PurchasedTicketDto> Tickets
        {
            get;
            set;
        } = [];
    }
}
