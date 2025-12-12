using FallingDetectionService.Api.Dtos.Sites;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/sites")]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _siteService;

        public SitesController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSiteRequestDto request)
        {
            try
            {
                var site = await _siteService.CreateAsync(request.Name, request.Location);

                var response = new SiteResponseDto
                {
                    Id = site.Id,
                    Name = site.Name,
                    Location = site.Location
                };

                return CreatedAtAction(nameof(GetById), new { id = site.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var site = await _siteService.GetAsync(id);
            if (site == null) return NotFound();

            var response = new SiteResponseDto
            {
                Id = site.Id,
                Name = site.Name,
                Location = site.Location
            };

            return Ok(response);
        }
    }
}