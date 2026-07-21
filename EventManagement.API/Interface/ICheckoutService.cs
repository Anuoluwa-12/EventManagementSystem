using EventManagement.API.DTO;
namespace EventManagement.API.Interface;
public interface ICheckoutService
{
    Task<InitializeCheckoutResponseDto>InitializeAsync(InitializeCheckoutRequestDto request);

    Task<VerifyPaymentResponseDto>VerifyAsync(string reference, int userId);
}