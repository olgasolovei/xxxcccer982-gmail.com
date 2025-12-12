using FallingDetectionService.Api.ViewModels.Zones;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.ControllersUi
{
    public class ZonesUiController : Controller
    {
        private readonly IZoneService _zoneService;

        public ZonesUiController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet]
        public IActionResult Create(Guid siteId) => View(new CreateZoneVm { SiteId = siteId });

        [HttpPost]
        public async Task<IActionResult> Create(CreateZoneVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var zone = await _zoneService.CreateAsync(vm.Name, vm.SiteId);
                return RedirectToAction(nameof(Index), new { id = zone.Id });
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {
            var zone = await _zoneService.GetAsync(id);
            if (zone == null) return NotFound();
            return View(zone);
        }
    }
}