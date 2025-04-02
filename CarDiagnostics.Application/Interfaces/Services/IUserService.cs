using CarDiagnostics.Application.DTOs.Users;
using Common.Helpers;

namespace CarDiagnostics.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetPagedUsers(int pageNumber, int pageSize);
        Task<UserDto?> GetUserById(int id);
        Task<UserDto?> GetUserByEmail(string email);
        Task<bool> UpdateUser(int id, UpdateUserDto userDto);
        Task<bool> DeleteUser(int id);
    }
}
