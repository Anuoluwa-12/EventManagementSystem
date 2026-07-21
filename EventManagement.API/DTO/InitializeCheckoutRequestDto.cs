using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.DTO
{
    public class InitializeCheckoutRequestDto
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } =
            string.Empty;

        [Required]
        public string CallbackUrl { get; set; } =
            string.Empty;

        [MinLength(1)]
        public List<CheckoutItemRequestDto> Items
        {
            get;
            set;
        } = [];
    }

}
