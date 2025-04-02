using CarDiagnostics.Application.DTOs.Base;

namespace CarDiagnostics.Application.DTOs.Users
{
    public class UserDto : BaseDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
