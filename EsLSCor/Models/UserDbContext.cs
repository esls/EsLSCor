using Microsoft.EntityFrameworkCore;

namespace EsLSCor.Models
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbUserModel> Users { get; set; }
    }
}
