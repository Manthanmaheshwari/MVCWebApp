using FirstMVCWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstMVCWebApp.Data
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product Price to use a decimal column type with appropriate precision.
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");

            // Configure unique index constraint on the User Email address.
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(true);
        }
    }
}