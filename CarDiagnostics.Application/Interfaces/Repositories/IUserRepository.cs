using CarDiagnostics.Domain.Entities.Users;

namespace CarDiagnostics.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetPagedUsers(int pageNumber, int pageSize);
        Task<UserEntity?> GetByEmail(string email);
        Task<UserEntity?> GetById(int id);
        Task AddUser(UserEntity user);
        Task UpdateUser(UserEntity user);
        Task DeleteUser(int id);
        Task<int> GetTotalUserCount();
    }
}
