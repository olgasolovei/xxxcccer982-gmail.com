using FallingDetectionService.Domain;
using Microsoft.EntityFrameworkCore;

namespace FallingDetectionService.Infrastructure.Persistence
{
    public class SafetyDbContext : DbContext
    {
        public SafetyDbContext(DbContextOptions<SafetyDbContext> options) : base(options)
        {
        }

        public DbSet<Site> Sites => Set<Site>();
        public DbSet<Zone> Zones => Set<Zone>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<Alert> Alerts => Set<Alert>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Унікальний SourceId в Device
            modelBuilder.Entity<Device>()
                .HasIndex(d => d.SourceId)
                .IsUnique();

            // Зв'язки
            modelBuilder.Entity<Zone>()
                .HasOne(z => z.Site)
                .WithMany(s => s.Zones)
                .HasForeignKey(z => z.SiteId);

            modelBuilder.Entity<Device>()
                .HasOne(d => d.Site)
                .WithMany(s => s.Devices)
                .HasForeignKey(d => d.SiteId);

            modelBuilder.Entity<SensorReading>()
                .HasOne(r => r.Device)
                .WithMany(d => d.SensorReadings)
                .HasForeignKey(r => r.DeviceId);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Zone)
                .WithMany(z => z.Incidents)
                .HasForeignKey(i => i.ZoneId);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Incident)
                .WithMany(i => i.Alerts)
                .HasForeignKey(a => a.IncidentId);
        }
    }
}