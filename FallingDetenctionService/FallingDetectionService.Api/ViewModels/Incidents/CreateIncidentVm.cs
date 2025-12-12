using System.ComponentModel.DataAnnotations;
using FallingDetectionService.Domain;

namespace FallingDetectionService.Api.ViewModels.Incidents
{
    public class CreateIncidentVm
    {
        [Required] public Guid SiteId { get; set; }
        [Required] public Guid ZoneId { get; set; }

        [Required] public string SourceId { get; set; } = default!;
        [Required] public string Type { get; set; } = "FALL"; // FALL / FALL_RISK

        [Range(0, 10)] public float Confidence { get; set; } = 0f;

        [Required] public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required] public string EvidenceRef { get; set; } = "demo://evidence";

        // для dropdown
        public IReadOnlyList<Site> Sites { get; set; } = Array.Empty<Site>();
        public IReadOnlyList<FallingDetectionService.Domain.Zone> Zones { get; set; }
            = Array.Empty<FallingDetectionService.Domain.Zone>();
        public IReadOnlyList<Device> Devices { get; set; } = Array.Empty<Device>();
    }
}