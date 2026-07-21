using EventManagement.API.DTO;
using EventManagement.API.DTO.Payment;

namespace EventManagement.API.Interface;
public interface IPaystackService
{
    Task<PaystackInitializeResponse?>InitializeTransactionAsync( PaystackInitializeRequest request);

    Task<PaystackVerifyResponse?>VerifyTransactionAsync(string reference);
}