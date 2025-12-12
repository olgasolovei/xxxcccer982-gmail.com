namespace FallingDetectionService.Api.Dtos.Devices;

public class DeviceRequestDto
{
    public string SourceId { get; set; } = default!;
    public string Type { get; set; } = default!;   // "AI", "EDGE", "GATEWAY"
    public Guid SiteId { get; set; }
    public string Metadata { get; set; } = string.Empty;
}