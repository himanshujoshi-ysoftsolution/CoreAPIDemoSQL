using CoreAPIDemo.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace CoreAPIDemo.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSet properties for each entity
        public DbSet<User> Users { get; set; }

        // Configure other entities and relationships here
        // ...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity mappings and relationships here
            // ...
        }
    }
}
