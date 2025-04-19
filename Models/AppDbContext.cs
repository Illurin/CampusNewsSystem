using Microsoft.EntityFrameworkCore;

namespace CampusNewsSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
		public DbSet<News> News { get; set; }
	}
}
