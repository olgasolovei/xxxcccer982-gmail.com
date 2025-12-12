using System;
using System.ComponentModel.DataAnnotations;

namespace FallingDetectionService.Api.Dtos.Zones
{
    public class CreateZoneRequestDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public Guid SiteId { get; set; }
    }
}