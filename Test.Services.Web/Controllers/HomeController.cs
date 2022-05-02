using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Test.Services.APITest2.Services;
using Test.Services.Web.Models;
using Test.Services.Web.Services;

namespace Test.Services.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestService _testService;
        private readonly ITestService2 _testService2;

        public HomeController(ILogger<HomeController> logger, ITestService testService, ITestService2 testService2)
        {
            _logger = logger;
            _testService = testService;
            _testService2 = testService2;
        }

        public async Task<IActionResult> Index()
        {
            var test = await _testService.GetAll();
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var test2 = await _testService2.GetAll();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}