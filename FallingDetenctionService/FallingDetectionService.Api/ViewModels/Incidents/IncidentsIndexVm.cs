using FallingDetectionService.Domain;

namespace FallingDetectionService.Api.ViewModels.Incidents
{
    public class IncidentsIndexVm
    {
        public Guid? SiteId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public IReadOnlyList<Site> Sites { get; set; } = Array.Empty<Site>();
        public IReadOnlyList<Incident> Incidents { get; set; } = Array.Empty<Incident>();

        public string? Message { get; set; }
    }
}