using CarDiagnostics.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CarDiagnostics.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
    }
}
