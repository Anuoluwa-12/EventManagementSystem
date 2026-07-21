namespace EventManagementSystem.DTO
{
    public class CheckoutRequestDto
    {
        public int UserId { get; set; }
        public string CustomerName { get; set; } =  string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public List<CheckoutItemDto> Items { get; set; } = [];
    }
}
