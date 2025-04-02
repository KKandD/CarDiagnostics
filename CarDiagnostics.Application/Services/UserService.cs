using CarDiagnostics.Application.DTOs.Users;
using CarDiagnostics.Application.Interfaces.Repositories;
using CarDiagnostics.Application.Interfaces.Services;
using Common.Helpers;

namespace CarDiagnostics.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResult<UserDto>> GetPagedUsers(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetPagedUsers(pageNumber, pageSize);
            var totalCount = await _userRepository.GetTotalUserCount();

            return new PagedResult<UserDto>
            {
                Items = users.Select(u => new UserDto { Id = u.Id, Email = u.Email }).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            return user == null ? null : new UserDto { Id = user.Id, Email = user.Email };
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            var user = await _userRepository.GetById(id);
            return user == null ? null : new UserDto { Id = user.Id, Email = user.Email };
        }

        public async Task<bool> UpdateUser(int id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(userDto.Email)) user.Email = userDto.Email;
            if (!string.IsNullOrEmpty(userDto.Password)) user.Password = PasswordHelper.HashPassword(userDto.Password);

            await _userRepository.UpdateUser(user);

            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);
            return true;
        }
    }
}
