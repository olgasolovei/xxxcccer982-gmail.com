using FallingDetectionService.Api.ViewModels.Incidents;
using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.ControllersUi
{
    public class IncidentsUiController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IZoneService _zoneService;
        private readonly IDeviceService _deviceService;
        private readonly IIncidentService _incidentService;
        private readonly IReportService _reportService;

        public IncidentsUiController(
            ISiteService siteService,
            IZoneService zoneService,
            IDeviceService deviceService,
            IIncidentService incidentService,
            IReportService reportService)
        {
            _siteService = siteService;
            _zoneService = zoneService;
            _deviceService = deviceService;
            _incidentService = incidentService;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? siteId, DateTime? start, DateTime? end)
        {
            DateTime ToUtc(DateTime dt)
            {
                // datetime-local приходить як Unspecified -> трактуємо як UTC (для демо)
                if (dt.Kind == DateTimeKind.Unspecified)
                    return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                return dt.ToUniversalTime();
            }

            var vm = new IncidentsIndexVm
            {
                Sites = await _siteService.ListAsync(),
                SiteId = siteId,
                Start = ToUtc(start ?? DateTime.UtcNow.Date.AddDays(-7)),
                End = ToUtc(end ?? DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1))
            };

            if (siteId.HasValue && siteId.Value != Guid.Empty)
            {
                vm.Incidents = await _incidentService.ListIncidentsAsync(siteId.Value, vm.Start, vm.End);
                if (vm.Incidents.Count == 0) vm.Message = "За вибраний період інцидентів немає.";
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? siteId)
        {
            var sites = await _siteService.ListAsync();
            var selectedSiteId = siteId ?? sites.FirstOrDefault()?.Id ?? Guid.Empty;

            var vm = new CreateIncidentVm
            {
                Sites = sites,
                SiteId = selectedSiteId
            };

            if (selectedSiteId != Guid.Empty)
            {
                vm.Zones = await _zoneService.ListBySiteAsync(selectedSiteId);
                vm.Devices = await _deviceService.ListBySiteAsync(selectedSiteId);
                vm.SourceId = vm.Devices.FirstOrDefault()?.SourceId ?? "";
                vm.ZoneId = vm.Zones.FirstOrDefault()?.Id ?? Guid.Empty;
            }

            return View(vm);
        }

        // простий “reload” dropdown-ів при зміні Site
        [HttpGet]
        public async Task<IActionResult> CreateForSite(Guid siteId)
        {
            return RedirectToAction(nameof(Create), new { siteId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateIncidentVm vm)
        {
            // треба підвантажити dropdown-и знову
            vm.Sites = await _siteService.ListAsync();
            vm.Zones = vm.SiteId == Guid.Empty ? Array.Empty<Domain.Zone>() : await _zoneService.ListBySiteAsync(vm.SiteId);
            vm.Devices = vm.SiteId == Guid.Empty ? Array.Empty<Domain.Device>() : await _deviceService.ListBySiteAsync(vm.SiteId);

            if (!ModelState.IsValid) return View(vm);

            if (!Enum.TryParse<IncidentType>(vm.Type, true, out var incidentType))
            {
                ModelState.AddModelError("", "Invalid incident type.");
                return View(vm);
            }

            try
            {
                var payload = new IncidentIngestPayload
                {
                    SiteId = vm.SiteId,
                    ZoneId = vm.ZoneId,
                    SourceId = vm.SourceId,
                    Timestamp = vm.Timestamp,
                    Type = incidentType,
                    Confidence = vm.Confidence,
                    EvidenceRef = vm.EvidenceRef
                };

                var incident = await _incidentService.IngestAsync(payload);
                return RedirectToAction(nameof(Details), new { id = incident.Id });
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var incident = await _incidentService.GetIncidentAsync(id);
            if (incident == null) return NotFound();
            return View(incident);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReport(Guid siteId, DateTime start, DateTime end, string format = "csv")
        {
            var bytes = await _reportService.GenerateSafetyReportAsync(siteId, start, end, format);

            if (bytes.Length == 0)
                return NoContent();

            if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
                return File(bytes, "text/csv", "safety-report.csv");

            return File(bytes, "application/json", "safety-report.json");
        }
    }
}
