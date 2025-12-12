
using System;
using System.Threading.Tasks;
using FallingDetectionService.Api.Dtos.Incidents;
using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/incidents")]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncident([FromBody] IncidentRequestDto request)
        {
            try
            {
                // Просте перетворення string -> enum; при помилці – 400
                if (!Enum.TryParse<IncidentType>(request.Type, ignoreCase: true, out var incidentType))
                {
                    return BadRequest("Invalid incident type");
                }

                var payload = new IncidentIngestPayload
                {
                    SiteId = request.SiteId,
                    ZoneId = request.ZoneId,
                    Timestamp = request.Timestamp,
                    Type = incidentType,
                    Confidence = request.Confidence,
                    SourceId = request.SourceId,
                    EvidenceRef = request.EvidenceRef
                };

                var incident = await _incidentService.IngestAsync(payload);

                var response = new IncidentResponseDto
                {
                    IncidentId = incident.Id
                };

                return CreatedAtAction(nameof(GetIncidentById), new { id = incident.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error"); // 5xx
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetIncidentById(Guid id)
        {
            var incident = await _incidentService.GetIncidentAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            return Ok(incident); // Для простоти віддаємо доменну модель
        }
    }
}
