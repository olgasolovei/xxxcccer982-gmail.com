using System;
using System.Threading.Tasks;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("safety")]
        public async Task<IActionResult> GetSafetyReport(
            [FromQuery] Guid siteId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] string format = "csv")
        {
            var data = await _reportService.GenerateSafetyReportAsync(siteId, start, end, format);

            if (data.Length == 0)
            {
                // Немає даних → 204
                return NoContent();
            }

            if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                return File(data, "text/csv", "safety-report.csv");
            }

            return File(data, "application/json", "safety-report.json");
        }
    }
}