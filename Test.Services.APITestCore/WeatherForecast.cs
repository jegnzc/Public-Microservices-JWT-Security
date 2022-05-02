namespace Test.Services.APITestCore
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 999999;

        public string? Summary => "Este es un servicio del banco.";
    }
}