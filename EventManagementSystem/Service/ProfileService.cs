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
            var response = await _httpClient.PutAsJsonAsync(
                $"https://localhost:7053/api/Profile/{userId}",
                dto);

            return response.IsSuccessStatusCode;
        }
    }
}