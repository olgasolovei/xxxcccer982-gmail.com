using Microsoft.AspNetCore.Mvc;

namespace FallingDetectionService.Api.ControllersUi
{
    public class DashboardController : Controller
    {
        public IActionResult Index() => View();
    }
}