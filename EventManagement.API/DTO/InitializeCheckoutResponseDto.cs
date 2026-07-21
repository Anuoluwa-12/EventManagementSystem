namespace EventManagement.API.DTO
{
    public class InitializeCheckoutResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AuthorizationUrl { get; set; }
        public string? Reference { get; set; }
    }
}
