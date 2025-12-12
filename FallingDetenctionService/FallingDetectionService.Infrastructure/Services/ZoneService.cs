using FallingDetectionService.Domain;

using FallingDetectionService.Infrastructure.Interfaces;
using FallingDetectionService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FallingDetectionService.Infrastructure.Services
{
    public class ZoneService : IZoneService
    {
        private readonly SafetyDbContext _db;

        public ZoneService(SafetyDbContext db)
        {
            _db = db;
        }

        public async Task<Zone> CreateAsync(string name, Guid siteId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (siteId == Guid.Empty) throw new ArgumentException("SiteId is required.");

            // Критично: перевіряємо, що Site існує, інакше FK впаде
            var siteExists = await _db.Sites.AnyAsync(s => s.Id == siteId);
            if (!siteExists)
                throw new KeyNotFoundException($"Site {siteId} not found.");

            var zone = new Zone
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                SiteId = siteId
            };

            _db.Zones.Add(zone);
            await _db.SaveChangesAsync();
            return zone;
        }
        
        public async Task<IReadOnlyList<Zone>> ListBySiteAsync(Guid siteId)
        {
            return await _db.Zones
                .Where(z => z.SiteId == siteId)
                .OrderBy(z => z.Name)
                .ToListAsync();
        }

        public Task<Zone?> GetAsync(Guid id)
        {
            return _db.Zones.FirstOrDefaultAsync(z => z.Id == id);
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            return _db.Zones.AnyAsync(z => z.Id == id);
        }
    }
}