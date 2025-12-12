namespace FallingDetectionService.Api.Dtos.Incidents;

public class IncidentRequestDto
{
    public Guid SiteId { get; set; }
    public Guid ZoneId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = default!;
    public float Confidence { get; set; }
    public string SourceId { get; set; } = default!;
    public string EvidenceRef { get; set; } = default!;
}