using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FallingDetectionService.Infrastructure.Interfaces;

namespace FallingDetectionService.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IIncidentService _incidentService;

        public ReportService(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        public async Task<byte[]> GenerateSafetyReportAsync(Guid siteId, DateTime start, DateTime end, string format)
        {
            var incidents = await _incidentService.ListIncidentsAsync(siteId, start, end);

            if (!incidents.Any())
            {
                // Порожній масив байтів => можна повернути 204 у контролері
                return Array.Empty<byte>();
            }

            format = format?.ToLowerInvariant();

            if (format == "csv")
            {
                var sb = new StringBuilder();
                sb.AppendLine("IncidentId,SiteId,ZoneId,Timestamp,Type,Confidence,Status,EvidenceRef");

                foreach (var i in incidents)
                {
                    sb.AppendLine($"{i.Id},{i.SiteId},{i.ZoneId},{i.Timestamp:o},{i.Type},{i.Confidence},{i.Status},{i.EvidenceRef}");
                }

                return Encoding.UTF8.GetBytes(sb.ToString());
            }

            // Простий JSON – без залежності від ASP.NET (якщо треба – можна використати System.Text.Json)
            var json = System.Text.Json.JsonSerializer.Serialize(incidents);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}