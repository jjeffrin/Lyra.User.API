using Lyra.UserService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Lyra.UserService.API.DataAccess
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
