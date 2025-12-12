using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using FallingDetectionService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FallingDetectionService.Infrastructure.Services
{
    public class SiteService : ISiteService
    {
        private readonly SafetyDbContext _db;

        public SiteService(SafetyDbContext db)
        {
            _db = db;
        }

        public async Task<Site> CreateAsync(string name, string location)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Location is required.");

            var site = new Site
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Location = location.Trim()
            };

            _db.Sites.Add(site);
            await _db.SaveChangesAsync();
            return site;
        }
        public async Task<IReadOnlyList<Site>> ListAsync()
        {
            return await _db.Sites
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public Task<Site?> GetAsync(Guid id)
        {
            return _db.Sites.FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            return _db.Sites.AnyAsync(s => s.Id == id);
        }
    }
}