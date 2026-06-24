using EventManagement.MVC.Models;
using EventManagementSystem.DTO;
namespace   EventManagementSystem.Interface;
public interface IAuthService
{
Task<bool> RegisterAsync(RegisterDto dto);Task<LoginResponseDto> LoginAsync(LoginDto dto);}