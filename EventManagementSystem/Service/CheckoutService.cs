using System.Net.Http.Json;
using System.Text.Json;
using EventManagementSystem.DTO;
using EventManagementSystem.Interface;

namespace EventManagementSystem.Service;

public class CheckoutService : ICheckoutService
{
    private readonly HttpClient _httpClient;

    public CheckoutService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InitializeCheckoutResponseDto> InitializeAsync(InitializeCheckoutRequestDto request)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/checkout/initialize", request);

        return await ReadResponseAsync< InitializeCheckoutResponseDto>(response);
    }

    public async Task<VerifyPaymentResponseDto> VerifyAsync(string reference, int userId)
    {
        using var response = await _httpClient.GetAsync("api/checkout/verify/" + Uri.EscapeDataString(reference) + $"?userId={userId}");

        return await ReadResponseAsync <VerifyPaymentResponseDto>(response);
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
        where T : class, new()
    {
        var body = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return new T();
        }

        return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        )
        ?? new T();
    }
}