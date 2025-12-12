using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FallingDetectionService.Domain;

namespace FallingDetectionService.Infrastructure.Interfaces
{
    public interface IIncidentService
    {
        Task<Incident> IngestAsync(IncidentIngestPayload payload);
        Task<Incident?> GetIncidentAsync(Guid id);
        Task<IReadOnlyList<Incident>> ListIncidentsAsync(Guid siteId, DateTime start, DateTime end);
    }

    public interface IDeviceService
    {
        Task<Device> CreateDeviceAsync(string sourceId, DeviceType type, Guid siteId, string metadata);
        Task<Device?> GetDeviceAsync(Guid id);
        
        Task<IReadOnlyList<Device>> ListBySiteAsync(Guid siteId);

    }

    public interface IReportService
    {
        Task<byte[]> GenerateSafetyReportAsync(Guid siteId, DateTime start, DateTime end, string format);
    }
    public interface ISiteService
    {
        Task<Site> CreateAsync(string name, string location);
        Task<Site?> GetAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        
        Task<IReadOnlyList<Site>> ListAsync();
    }
    public interface IZoneService
    {
        Task<Zone> CreateAsync(string name, Guid siteId);
        Task<Zone?> GetAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        
        Task<IReadOnlyList<Zone>> ListBySiteAsync(Guid siteId);

    }
    
    public class IncidentIngestPayload
    {
        public Guid SiteId { get; set; }
        public Guid ZoneId { get; set; }
        public string SourceId { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public IncidentType Type { get; set; }
        public float Confidence { get; set; }
        public string EvidenceRef { get; set; } = default!;
    }
}