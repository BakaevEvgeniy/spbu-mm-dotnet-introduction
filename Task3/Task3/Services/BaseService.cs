using RestSharp;
using WeatherService.DataModel;

namespace WeatherService.Services
{
    public abstract class BaseService
    {
        public string Name { get; protected set; }
        public string ApiKey_ { get; private set; }
        protected RestClient restClient_;
        protected RestRequest getWeatherRequest;
        public abstract WeatherData Request();
        public abstract void SetLatitudeAndLongitude(double lat, double lng);
        protected BaseService(string apiKey)
        {
            ApiKey_ = apiKey;
        }
    }
}