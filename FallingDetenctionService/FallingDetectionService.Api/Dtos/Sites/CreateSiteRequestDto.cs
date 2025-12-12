using System.ComponentModel.DataAnnotations;

namespace FallingDetectionService.Api.Dtos.Sites
{
    public class CreateSiteRequestDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Location { get; set; } = default!;
    }
}