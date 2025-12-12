using System;
using System.Threading.Tasks;
using FallingDetectionService.Api.Dtos.Devices;
using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody] DeviceRequestDto request)
        {
            try
            {
                if (!Enum.TryParse<DeviceType>(request.Type, true, out var deviceType))
                {
                    return BadRequest("Invalid device type");
                }

                var device = await _deviceService.CreateDeviceAsync(
                    request.SourceId,
                    deviceType,
                    request.SiteId,
                    request.Metadata);

                var response = new DeviceResponseDto
                {
                    Id = device.Id,
                    SourceId = device.SourceId
                };

                return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                // sourceId вже існує → 409
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDeviceById(Guid id)
        {
            var device = await _deviceService.GetDeviceAsync(id);
            if (device == null)
                return NotFound();

            return Ok(device);
        }
    }
}
