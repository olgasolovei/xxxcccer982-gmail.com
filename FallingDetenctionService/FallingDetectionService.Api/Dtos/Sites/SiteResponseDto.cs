namespace FallingDetectionService.Api.Dtos.Sites
{
    public class SiteResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
    }
}