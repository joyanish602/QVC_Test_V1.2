using Microsoft.EntityFrameworkCore;

namespace UserWebAPI.Model
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Users> users { get; set; }
        public DbSet<UserGroups> userGroups { get; set; }
    }
}
