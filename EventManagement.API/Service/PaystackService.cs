using EventManagement.API.DTO;
using EventManagement.API.DTO.Payment;
using EventManagement.API.Interface;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EventManagement.API.Service;

public class PaystackService : IPaystackService
{
    private readonly HttpClient _httpClient;

    public PaystackService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        var secretKey =
            configuration["Paystack:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "Paystack:SecretKey is missing."
            );
        }

        _httpClient.DefaultRequestHeaders
            .Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                secretKey
            );

        _httpClient.DefaultRequestHeaders
            .Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"
                )
            );
    }

    public async Task<PaystackInitializeResponse?>InitializeTransactionAsync(PaystackInitializeRequest request)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "transaction/initialize",
                request
            );

        var body =
            await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        return JsonSerializer.Deserialize<PaystackInitializeResponse>
            (
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
    }

    public async Task<PaystackVerifyResponse?> VerifyTransactionAsync(string reference)
    {
        using var response =
            await _httpClient.GetAsync(
                "transaction/verify/" +
                Uri.EscapeDataString(reference)
            );

        var body =
            await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        return JsonSerializer.Deserialize<PaystackVerifyResponse>
            (
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
    }
}