namespace FallingDetectionService.Api.Dtos.Devices;

public class DeviceResponseDto
{
    public Guid Id { get; set; }
    public string SourceId { get; set; } = default!;
}