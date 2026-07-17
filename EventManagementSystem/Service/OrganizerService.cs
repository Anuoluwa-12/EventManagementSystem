using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EventManagementSystem.Interface;
using EventManagementSystem.Models;

namespace EventManagementSystem.Service;

public class OrganizerService : IOrganizerService
{
    private readonly HttpClient _httpClient;

    public OrganizerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RegisterAsync(string token, OrganizerRegistrationViewModel model)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Post,
                "api/organizer/register",
                token
            );

        request.Content =
            JsonContent.Create(model);

        using var response =
            await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    public async Task<OrganizerDashboardViewModel?>GetDashboardAsync(string token)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Get,
                "api/organizer/dashboard",
                token
            );

        using var response =
            await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<
                OrganizerDashboardViewModel
            >();
    }

    public async Task<List<OrganizerEventViewModel>>GetEventsAsync(string token)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Get,
                "api/organizer/events",
                token
            );

        using var response =
            await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new List<OrganizerEventViewModel>();
        }

        return await response.Content
            .ReadFromJsonAsync<List<OrganizerEventViewModel>>()?? new List<OrganizerEventViewModel>();
    }

    public async Task<OrganizerEventViewModel?>
        GetEventAsync(string token, int eventId)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Get,
                $"api/organizer/events/{eventId}",
                token
            );

        using var response =
            await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<OrganizerEventViewModel>();
    }

    public async Task<bool> CreateEventAsync(string token, OrganizerEventFormViewModel model)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Post,
                "api/organizer/events",
                token
            );

        using var formContent =
            CreateEventFormContent(model);

        request.Content = formContent;

        using var response =
            await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEventAsync(string token, int eventId, OrganizerEventFormViewModel model)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Put,
                $"api/organizer/events/{eventId}",
                token
            );

        using var formContent =
            CreateEventFormContent(model);

        request.Content = formContent;

        using var response =
            await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CancelEventAsync(string token, int eventId)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Patch,
                $"api/organizer/events/{eventId}/cancel",
                token
            );

        request.Content =
            new StringContent(string.Empty);

        using var response =
            await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEventAsync(string token, int eventId)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Delete,
                $"api/organizer/events/{eventId}",
                token
            );

        using var response =
            await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    private static HttpRequestMessage
        CreateAuthorizedRequest(HttpMethod method, string url, string token)
    {
        var request =
            new HttpRequestMessage(method, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token
            );

        return request;
    }

    private static MultipartFormDataContent
        CreateEventFormContent(OrganizerEventFormViewModel model)
    {
        var content =
            new MultipartFormDataContent();

        content.Add(
            new StringContent(model.Title),
            nameof(model.Title)
        );

        content.Add(
            new StringContent(model.Description),
            nameof(model.Description)
        );

        content.Add(
            new StringContent(
                model.EventDate.ToString(
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture
                )
            ),
            nameof(model.EventDate)
        );

        content.Add(
            new StringContent(
                model.EventTime.ToString(
                    @"hh\:mm\:ss"
                )
            ),
            nameof(model.EventTime)
        );

        content.Add(
            new StringContent(model.Venue),
            nameof(model.Venue)
        );

        content.Add(
            new StringContent(
                model.TicketPrice.ToString(
                    CultureInfo.InvariantCulture
                )
            ),
            nameof(model.TicketPrice)
        );

        content.Add(
            new StringContent(
                model.TotalSeats.ToString()
            ),
            nameof(model.TotalSeats)
        );

        content.Add(
            new StringContent(model.Category),
            nameof(model.Category)
        );

        if (model.EventImage is not null)
        {
            var imageContent =
                new StreamContent(
                    model.EventImage.OpenReadStream()
                );

            imageContent.Headers.ContentType =
                new MediaTypeHeaderValue(
                    model.EventImage.ContentType
                );

            content.Add(
                imageContent,
                nameof(model.EventImage),
                model.EventImage.FileName
            );
        }

        return content;
    }
}