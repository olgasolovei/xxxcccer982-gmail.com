using System.ComponentModel.DataAnnotations;

namespace FallingDetectionService.Api.ViewModels.Sites
{
    public class CreateSiteVm
    {
        [Required] public string Name { get; set; } = default!;
        [Required] public string Location { get; set; } = default!;
    }
}