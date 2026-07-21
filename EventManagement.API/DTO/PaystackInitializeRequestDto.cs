using System.Text.Json.Serialization;

namespace EventManagement.API.DTO.Payment;

public class PaystackInitializeRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "NGN";

    [JsonPropertyName("metadata")]
    public string? Metadata { get; set; }
}




