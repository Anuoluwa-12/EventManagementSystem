using EventManagementSystem.DTO;

namespace EventManagementSystem.Interface;

public interface ICheckoutService
{
    Task<InitializeCheckoutResponseDto>InitializeAsync(InitializeCheckoutRequestDto request);

    Task<VerifyPaymentResponseDto>VerifyAsync(string reference, int userId);
}