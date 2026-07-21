namespace EventManagementSystem.DTO
{
    public class InitializeCheckoutRequestDto
    {
        public int UserId { get; set; }

        public string Email { get; set; } =
            string.Empty;

        public string CallbackUrl { get; set; } =
            string.Empty;

        public List<CheckoutItemRequestDto> Items
        {
            get;
            set;
        } = [];
    }

}
