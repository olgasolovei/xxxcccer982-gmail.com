using FallingDetectionService.Api.ViewModels.Sites;
using FallingDetectionService.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.ControllersUi
{
    public class SitesUiController : Controller
    {
        private readonly ISiteService _siteService;

        public SitesUiController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        // /SitesUi  -> список всіх
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sites = await _siteService.ListAsync();
            return View(sites); // Views/SitesUi/Index.cshtml -> таблиця
        }

        // /SitesUi/Details/{id} -> деталі одного site
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var site = await _siteService.GetAsync(id);
            if (site == null) return NotFound();

            return View(site); // Views/SitesUi/Details.cshtml
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateSiteVm());

        [HttpPost]
        public async Task<IActionResult> Create(CreateSiteVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var site = await _siteService.CreateAsync(vm.Name, vm.Location);

            // після створення логічніше перейти на Details
            return RedirectToAction(nameof(Index), new { id = site.Id });
        }

        
    }
}
