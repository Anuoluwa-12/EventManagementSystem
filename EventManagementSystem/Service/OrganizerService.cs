using EventManagementSystem.Interface;
using EventManagementSystem.Models;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EventManagementSystem.Service;

public class OrganizerService : IOrganizerService
{
    private readonly HttpClient _httpClient;

    public OrganizerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RegisterAsync(
     string token,
     OrganizerRegistrationViewModel model)
    {
        using var request =
            CreateAuthorizedRequest(
                HttpMethod.Post,
                "api/organizer/register",
                token
            );

        request.Content = JsonContent.Create(model);

        using var response =
            await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException(
                "Your login session has expired."
            );
        }

        var rawResponse =
            await response.Content.ReadAsStringAsync();

        // Log the technical response on the server only.
        Console.WriteLine(
            $"Organizer API returned {(int)response.StatusCode}: {rawResponse}"
        );

        var safeMessage =
            "The organizer profile could not be created. Please try again.";

        /*
         * Only display a message when the API returned
         * a proper JSON response. Never display HTML,
         * stack traces or Developer Exception Page content.
         */
        var mediaType =
            response.Content.Headers.ContentType?.MediaType;

        if (string.Equals(
            mediaType,
            "application/json",
            StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                mediaType,
                "application/problem+json",
                StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using var document =
                    JsonDocument.Parse(rawResponse);

                if (document.RootElement.TryGetProperty(
                    "message",
                    out var messageElement))
                {
                    var apiMessage =
                        messageElement.GetString();

                    if (!string.IsNullOrWhiteSpace(apiMessage))
                    {
                        safeMessage = apiMessage;
                    }
                }
                else if (document.RootElement.TryGetProperty(
                    "title",
                    out var titleElement))
                {
                    var apiTitle =
                        titleElement.GetString();

                    if (!string.IsNullOrWhiteSpace(apiTitle))
                    {
                        safeMessage = apiTitle;
                    }
                }
            }
            catch (JsonException)
            {
                // Do not expose malformed API content.
            }
        }

        throw new InvalidOperationException(safeMessage);
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