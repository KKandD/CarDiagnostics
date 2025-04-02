using CarDiagnostics.Domain.Entities.Base;

namespace CarDiagnostics.Domain.Entities.Users
{
    public class UserEntity : BaseEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
