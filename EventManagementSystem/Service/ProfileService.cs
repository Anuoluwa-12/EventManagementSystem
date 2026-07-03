using EventManagementSystem.DTO;
using EventManagementSystem.Interface;

namespace EventManagementSystem.Service
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProfileDto> GetProfileAsync(int userId)
        {
            return await _httpClient
                .GetFromJsonAsync<ProfileDto>(
                $"https://localhost:7053/api/Profile/{userId}");
        }

        public async Task<bool> UpdateProfileAsync(
      int userId,
      UpdateProfileDto dto)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.FirstName), "FirstName");
            content.Add(new StringContent(dto.LastName), "LastName");
            content.Add(new StringContent(dto.Email), "Email");

            if (dto.ProfilePicture != null)
            {
                var streamContent = new StreamContent(dto.ProfilePicture.OpenReadStream());

                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(dto.ProfilePicture.ContentType);

                content.Add(
                    streamContent,
                    "ProfilePicture",
                    dto.ProfilePicture.FileName);
            }

            var response = await _httpClient.PutAsync(
                $"https://localhost:7053/api/Profile/{userId}",
                content);

            return response.IsSuccessStatusCode;
        }
    }
}