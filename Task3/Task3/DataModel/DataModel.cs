    
namespace WeatherService.DataModel
{
    public class WeatherSourceData
    {
        private Dictionary<string, WeatherData> weatherSources_ = new Dictionary<string, WeatherData>();

        public void AddWeatherSourceData(string source, WeatherData weatherData)
        {
            weatherData.ValidateOutput();
            weatherSources_[source] = weatherData;
        }
        public Dictionary<string, WeatherData> GetWeatherSourceData()
        {
            return weatherSources_;
        }

    }
    public class WeatherData
    {
        public double TemperatureC { get; set; }
        public double TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string? Cloudiness { get; set; }
        public string? Humidity { get; set; }
        public string? Precipitation { get; set; }
        public string? WindDirection { get; set; }
        public string? WindSpeed { get; set; }

        public void ValidateOutput()
        {
            Cloudiness = Cloudiness != null ? Cloudiness : "NODATA";
            Humidity = Humidity != null ? Humidity : "NODATA";
            Precipitation = Precipitation != null ? Precipitation : "NODATA";
            WindDirection = WindDirection != null ? WindDirection : "NODATA";
            WindSpeed = WindSpeed != null ? WindSpeed : "NODATA";
        }
    }
}