using Microsoft.EntityFrameworkCore;

namespace AadhaarVerification.Models
{
    public class UserDataDbContext : DbContext
    {
        public UserDataDbContext(DbContextOptions<UserDataDbContext> options) : base(options) { }

        public DbSet<Data> Datas { get; set; }

        
    }
}
