using System;
using System.ComponentModel.DataAnnotations;

namespace FallingDetectionService.Api.ViewModels.Zones
{
    public class CreateZoneVm
    {
        [Required] public string Name { get; set; } = default!;
        [Required] public Guid SiteId { get; set; }
    }
}