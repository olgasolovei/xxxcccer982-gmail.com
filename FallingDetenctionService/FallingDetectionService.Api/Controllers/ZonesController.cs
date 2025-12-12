using FallingDetectionService.Api.Dtos.Zones;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/zones")]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZonesController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateZoneRequestDto request)
        {
            try
            {
                var zone = await _zoneService.CreateAsync(request.Name, request.SiteId);

                var response = new ZoneResponseDto
                {
                    Id = zone.Id,
                    Name = zone.Name,
                    SiteId = zone.SiteId
                };

                return CreatedAtAction(nameof(GetById), new { id = zone.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                // Site не існує
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var zone = await _zoneService.GetAsync(id);
            if (zone == null) return NotFound();

            var response = new ZoneResponseDto
            {
                Id = zone.Id,
                Name = zone.Name,
                SiteId = zone.SiteId
            };

            return Ok(response);
        }
    }
}