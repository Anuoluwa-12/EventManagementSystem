using EventManagement.API.DTO;
namespace EventManagement.API.Interface;
public interface IUserService
{
    Task<string> RegisterAsync(RegisterDto dto);

    Task<LoginResponseDto> LoginAsync(LoginDto dto);

    Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);

    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<string> CorporateOnboardingAsync(CorporateOnboardingDto dto);
    Task<ProfileDto> GetProfileAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId,
                              UpdateProfileDto dto);
    Task<bool> UpdateAccountTypeAsync(
    int userId,
    string accountType);

    Task<List<BookedEventDto>> GetBookedEventsAsync(int userId);

    Task<List<TicketEventDto>> GetTicketsAsync(int userId);
}