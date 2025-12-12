using System;
using System.ComponentModel.DataAnnotations;

namespace FallingDetectionService.Api.ViewModels.Devices
{
    public class CreateDeviceVm
    {
        [Required] public string SourceId { get; set; } = default!;
        [Required] public string Type { get; set; } = "AI"; // AI/EDGE/GATEWAY
        [Required] public Guid SiteId { get; set; }
        public string Metadata { get; set; } = "";
    }
}