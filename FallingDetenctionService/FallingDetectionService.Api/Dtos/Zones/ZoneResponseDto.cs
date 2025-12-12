using System;

namespace FallingDetectionService.Api.Dtos.Zones
{
    public class ZoneResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid SiteId { get; set; }
    }
}