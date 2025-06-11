using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServicePlanner.Models;

namespace ServicePlanner.Data
{
    public class ServicePlannerContext : IdentityDbContext<User>
    {
        public ServicePlannerContext(DbContextOptions<ServicePlannerContext> options) : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<ServiceTemplate> ServiceTemplates { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceEvent> ServiceEvents { get; set; }
        public DbSet<ServiceEventInstance> ServiceEventInstances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Song entity
            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SongName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Key).HasMaxLength(10);
                entity.Property(e => e.SongSelectId).HasMaxLength(50);
                entity.Property(e => e.Speed).HasMaxLength(50);
                entity.Property(e => e.Publisher).HasMaxLength(100);
                entity.Property(e => e.Artist).HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            // Configure ServiceTemplate entity
            modelBuilder.Entity<ServiceTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasMany(e => e.Events)
                      .WithOne()
                      .HasForeignKey(e => e.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Service entity
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Template)
                      .WithMany()
                      .HasForeignKey(e => e.TemplateId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.EventInstances)
                      .WithOne()
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ServiceEvent entity
            modelBuilder.Entity<ServiceEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Type).HasConversion<string>();
            });

            // Configure ServiceEventInstance entity
            modelBuilder.Entity<ServiceEventInstance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PersonName).HasMaxLength(100);
                entity.Property(e => e.SongTitle).HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.HasOne(e => e.ServiceEvent)
                      .WithMany()
                      .HasForeignKey(e => e.ServiceEventId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).HasConversion<string>();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
            });
        }
    }
}
