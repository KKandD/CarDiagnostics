using CarDiagnostics.Application.Interfaces.Repositories;
using CarDiagnostics.Domain.Entities.Users;
using CarDiagnostics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDiagnostics.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<UserEntity>> GetPagedUsers(int pageNumber, int pageSize)
        {
            return await _appDbContext.Users
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<UserEntity?> GetByEmail(string email)
        {
            return await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity?> GetById(int id)
        {
            return await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddUser(UserEntity user)
        {
            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateUser(UserEntity user)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _appDbContext.Users.FindAsync(id);
            if (user != null)
            {
                _appDbContext.Users.Remove(user);
                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalUserCount()
        {
            return await _appDbContext.Users.CountAsync();
        }
    }
}
