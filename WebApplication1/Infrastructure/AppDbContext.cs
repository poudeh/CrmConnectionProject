using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain;

namespace WebApplication1.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DefaultDescription> DefaultDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DefaultDescription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Owner).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Code).HasMaxLength(500);
            });
        }
    }
}
