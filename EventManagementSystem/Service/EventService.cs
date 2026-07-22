using System.Globalization;
using System.Net.Http.Json;
using EventManagementSystem.DTO;
using EventManagementSystem.Interface;

namespace EventManagementSystem.Service;

public class EventService : IEventService
{
    private const string ApiBaseUrl =
        "https://localhost:7053/api/Event";

    private readonly HttpClient _httpClient;

    public EventService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<EventDto>> GetAllEventsAsync(
        string? search,
        string? category,
        string? location,
        DateTime? date,
        decimal? maxPrice)
    {
        var queryParameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            queryParameters.Add(
                $"search={Uri.EscapeDataString(search)}"
            );
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            queryParameters.Add(
                $"category={Uri.EscapeDataString(category)}"
            );
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            queryParameters.Add(
                $"location={Uri.EscapeDataString(location)}"
            );
        }

        if (date.HasValue)
        {
            queryParameters.Add(
                $"date={date.Value:yyyy-MM-dd}"
            );
        }

        if (maxPrice.HasValue)
        {
            queryParameters.Add(
                $"maxPrice={maxPrice.Value.ToString(
                    CultureInfo.InvariantCulture
                )}"
            );
        }

        var url = ApiBaseUrl;

        if (queryParameters.Count > 0)
        {
            url += "?" + string.Join(
                "&",
                queryParameters
            );
        }

        return await _httpClient
            .GetFromJsonAsync<List<EventDto>>(url)
            ?? [];
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        using var response =
            await _httpClient.GetAsync(
                $"{ApiBaseUrl}/{id}"
            );

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<EventDto>();
    }

    public async Task<bool> BookEventAsync(
        int userId,
        int eventId)
    {
        if (userId <= 0 || eventId <= 0)
        {
            return false;
        }

        using var response =
            await _httpClient.PostAsync(
                $"{ApiBaseUrl}/{eventId}/book" +
                $"?userId={userId}",
                content: null
            );

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> IsEventBookedAsync(
        int userId,
        int eventId)
    {
        if (userId <= 0 || eventId <= 0)
        {
            return false;
        }

        try
        {
            return await _httpClient
                .GetFromJsonAsync<bool>(
                    $"{ApiBaseUrl}/{eventId}/is-booked" +
                    $"?userId={userId}"
                );
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}