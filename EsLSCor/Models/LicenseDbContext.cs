using Microsoft.EntityFrameworkCore;

namespace EsLSCor.Models
{
    public class LicenseDbContext : DbContext
    {
        public LicenseDbContext(DbContextOptions<LicenseDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbLicenseModel> Licenses { get; set; }
    }
}
