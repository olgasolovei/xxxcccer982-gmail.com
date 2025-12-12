using FallingDetectionService.Api.ViewModels.Devices;
using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.ControllersUi
{
    public class DevicesUiController : Controller
    {
        private readonly IDeviceService _deviceService;

        public DevicesUiController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public IActionResult Create(Guid siteId) => View(new CreateDeviceVm { SiteId = siteId });

        [HttpPost]
        public async Task<IActionResult> Create(CreateDeviceVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                if (!Enum.TryParse<FallingDetectionService.Domain.DeviceType>(vm.Type, true, out var type))
                {
                    ModelState.AddModelError("", "Invalid device type");
                    return View(vm);
                }

                var device = await _deviceService.CreateDeviceAsync(vm.SourceId, type, vm.SiteId, vm.Metadata);
                return RedirectToAction(nameof(Index), new { id = device.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message); // 409 scenario
                return View(vm);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message); // site not found if you added that check
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {
            var device = await _deviceService.GetDeviceAsync(id);
            if (device == null) return NotFound();
            return View(device);
        }
    }
}