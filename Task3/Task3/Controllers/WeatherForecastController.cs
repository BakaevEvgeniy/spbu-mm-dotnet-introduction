using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using WeatherService.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using WeatherService.DataModel;

namespace WeatherService.Controllers
{
    [Route("api/")]
    [ApiController]
    [Produces("application/json")]
    public class WeatherController : ControllerBase
    {
        private readonly Tommorow tommorowClient;
        private readonly StormGlass stormGlassClient;
        private readonly double lat = 59.93863;
        private readonly double lng = 30.31413;
        
        public WeatherController()
        {
            tommorowClient = new Tommorow(Environment.GetEnvironmentVariable("TommorowApiKey"));
            stormGlassClient = new StormGlass(Environment.GetEnvironmentVariable("StormGlassApiKey"));
            if (Environment.GetEnvironmentVariable("LATITUDE") != null)
            {
                lat = Convert.ToDouble(Environment.GetEnvironmentVariable("LATITUDE"));
            }

            if (Environment.GetEnvironmentVariable("LONGITUDE") != null)
            {
                lng = Convert.ToDouble(Environment.GetEnvironmentVariable("LONGITUDE"));
            }
        }
        private WeatherData SendRequest(BaseService service)
        {
            service.SetLatitudeAndLongitude(lat, lng);
            try
            {
                var response = service.Request();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"{service.Name}. Error: {ex.Message}");
            }
        }
        private WeatherSourceData GetWeatherViaApi(string source)
        {
            WeatherSourceData data = new WeatherSourceData();
            if (source.ToLower() == "stormglass")
            {
                data.AddWeatherSourceData("stormglass", SendRequest(stormGlassClient));
            }
            else if (source.ToLower() == "tommorow")
            {
                data.AddWeatherSourceData("tommorow", SendRequest(tommorowClient));
            }
            else if (source.ToLower() == "all")
            {
                data.AddWeatherSourceData("stormglass", SendRequest(stormGlassClient));
                data.AddWeatherSourceData("tommorow", SendRequest(tommorowClient));
            }
            else 
            {
                throw new Exception($"Unknown source: {source}");
            }

            return data;
        }

        /// <summary>
        /// Get weather data via source API. Available sources: StormGlass, Tommorow
        /// </summary>
        /// <param name="source"></param>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /Weather?source=stormglass
        ///     GET /Weather?source=tommorow
        ///     GET /Weather?source=all
        ///
        /// </remarks>
        /// <response code="200">Returns weather data as json</response>
        /// <response code="400">The source field is required.</response>
        /// <response code="500">Something wrong with source api!</response>
        [HttpGet("weather")]
        public IActionResult GetWeather(string source)
        {
            try
            {
                var weatherData = GetWeatherViaApi(source);
                return Ok(weatherData.GetWeatherSourceData());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get available sources
        /// </summary>
        [HttpGet("sources")]
        public IActionResult GetSources()
        {
            return Ok("StormGlass, Tommorow");
        }

    }
}
