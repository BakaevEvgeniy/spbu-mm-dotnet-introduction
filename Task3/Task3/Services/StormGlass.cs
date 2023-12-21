using Newtonsoft.Json;
using RestSharp;
using WeatherService.DataModel;

namespace WeatherService.Services
{
    public class StormGlass : BaseService
    {
        public class AirTemperature
        {
            public double dwd { get; set; }
            public double noaa { get; set; }
            public double sg { get; set; }
        }

        public class CloudCover
        {
            public double dwd { get; set; }
            public double noaa { get; set; }
            public double sg { get; set; }
        }

        public class Gust
        {
            public double dwd { get; set; }
            public double noaa { get; set; }
            public double sg { get; set; }
        }

        public class Hour
        {
            public AirTemperature airTemperature { get; set; }
            public CloudCover cloudCover { get; set; }
            public Gust gust { get; set; }
            public Humidity humidity { get; set; }
            public DateTime time { get; set; }
            public WindWaveDirection windWaveDirection { get; set; }
        }

        public class Humidity
        {
            public double dwd { get; set; }
            public double noaa { get; set; }
            public double sg { get; set; }
        }

        public class Meta
        {
            public int cost { get; set; }
            public int dailyQuota { get; set; }
            public string end { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public List<string> @params { get; set; }
            public int requestCount { get; set; }
            public string start { get; set; }
        }

        public class Root
        {
            public List<Hour> hours { get; set; }
            public Meta meta { get; set; }
        }

        public class WindWaveDirection
        {
            public double dwd { get; set; }
            public double icon { get; set; }
            public double sg { get; set; }
        }


        public StormGlass(string apiKey) : base(apiKey)
        {
            Name = "StormGlass";
            restClient_ = new RestClient("https://api.stormglass.io/v2/");
            getWeatherRequest = new RestRequest($"weather/point", Method.Get);
            getWeatherRequest.AddHeader("Authorization", ApiKey_);
            getWeatherRequest.AddQueryParameter("params", "airTemperature,cloudCover,humidity,gust,windWaveDirection");
        }

        public override WeatherData Request()
        {
            var response = restClient_.Execute(getWeatherRequest);
            if (response.IsSuccessful)
            {
                Root? weatherHistory = JsonConvert.DeserializeObject<Root>(response.Content);
                if (weatherHistory == null || weatherHistory.hours.Count == 0) {
                    throw new Exception($"Error: NODATA");
                }

                Hour now = weatherHistory.hours[0];
                foreach (var item in weatherHistory.hours)
                {
                    if (item.time.Hour == DateTime.UtcNow.Hour)
                    {
                        now = item;
                        break;
                    }
                }

                var resp = new WeatherData
                {
                    TemperatureC = now.airTemperature.noaa,
                    Cloudiness = now.cloudCover.noaa.ToString(),
                    Humidity = now.humidity.noaa.ToString(),
                    WindDirection = now.windWaveDirection.dwd.ToString(),
                    WindSpeed =  now.gust.noaa.ToString(),
                };
                return resp;
            }
            else
            {
                throw new Exception($"Error: {response.ErrorMessage}");
            }
        }
        public override void SetLatitudeAndLongitude(double lat, double lng)
        {
            getWeatherRequest.AddQueryParameter("lat", lat);
            getWeatherRequest.AddQueryParameter("lng", lng);
        }
    }
}

