using System;
using System.Threading.Tasks;
using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using FallingDetectionService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FallingDetectionService.Infrastructure.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly SafetyDbContext _db;

        public DeviceService(SafetyDbContext db)
        {
            _db = db;
        }

        public async Task<Device> CreateDeviceAsync(string sourceId, DeviceType type, Guid siteId, string metadata)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException("sourceId is required");

            // Перевірка на дублікат для 409
            var exists = await _db.Devices.AnyAsync(d => d.SourceId == sourceId);
            if (exists)
            {
                throw new InvalidOperationException("Device with this sourceId already exists");
            }

            var siteExists = await _db.Sites.AnyAsync(s => s.Id == siteId);
            if (!siteExists)
                throw new KeyNotFoundException($"Site {siteId} not found.");

            var device = new Device
            {
                Id = Guid.NewGuid(),
                SourceId = sourceId,
                Type = type,
                SiteId = siteId,
                Metadata = metadata ?? string.Empty
            };

            await _db.Devices.AddAsync(device);
            await _db.SaveChangesAsync();

            return device;
        }

        Task<Device?> IDeviceService.GetDeviceAsync(Guid id)
        {
            return GetDeviceAsync(id);
        }

        public async Task<Device?> GetDeviceAsync(Guid id)
        {
            return await _db.Devices.FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<IReadOnlyList<Device>> ListBySiteAsync(Guid siteId)
        {
            return await _db.Devices
                .Where(d => d.SiteId == siteId)
                .OrderBy(d => d.SourceId)
                .ToListAsync();
        }

    }
}