using EventManagement.API.DTO.Payment;
using System.Text.Json.Serialization;

namespace EventManagement.API.DTO
{
    public class PaystackInitializeResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public PaystackInitializeData? Data
        {
            get;
            set;
        }
    }
}