using CarDiagnostics.Application.DTOs.Base;
using System.ComponentModel.DataAnnotations;

namespace CarDiagnostics.Application.DTOs.Users
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 32 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[\W_]).+$", ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
