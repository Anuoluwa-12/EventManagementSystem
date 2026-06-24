using EventManagement.MVC.Models;
using EventManagementSystem.Interface;
using EventManagementSystem.DTO;
using System.Net.Http.Json;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "https://localhost:7053/api/Auth/register",
            dto);

        return response.IsSuccessStatusCode;
    }

public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
{
    var response = await _httpClient.PostAsJsonAsync(
        "https://localhost:7053/api/Auth/login",
        dto);

    if (!response.IsSuccessStatusCode)
        return null;

    return await response.Content
        .ReadFromJsonAsync<LoginResponseDto>();
}
}