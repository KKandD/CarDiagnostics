using CarDiagnostics.Application.DTOs.Users;

namespace CarDiagnostics.Application.Interfaces.Services
{
    public interface IAuthorizationService
    {
        Task<string> Register(CreateUserDto createuserDto);
        Task<string> Login(CreateUserDto createuserDto);
    }
}
