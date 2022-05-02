using Microsoft.AspNetCore.Mvc;
using Test.Services.Web.Services;

namespace Test.Services.APITest2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ITestCoreService _testCoreService;

        private static readonly string[] Summaries = new[]
        {
        "Test 2", "test 2", "TEST 2", "T e s t 2", "T e s t  2", "T e s t 2"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITestCoreService testCoreService)
        {
            _logger = logger;
            _testCoreService = testCoreService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var user = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var callCoreServiceTest = await _testCoreService.GetAll();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = 14234232,
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }
    }
}