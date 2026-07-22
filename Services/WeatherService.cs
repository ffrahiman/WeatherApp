using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services

/// <summary>
/// Handles all communication with the Open-Meteo API.
/// </summary>

{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary> Searches for cities matching the given name using the Geocoding API. </summary>
        /// <param name="cityName">The city name to search for.</param>
        /// <returns>A list of matching cities, or an empty list if none found.</returns>
        public async Task<List<City>> SearchCitiesAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return new List<City>();

            try
            {
                string url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(cityName)}&count=10&language=en&format=json";

                string json = await _httpClient.GetStringAsync(url);

                var response = JsonSerializer.Deserialize<GeocodingResponse>(json, JsonOptions);

                if (response?.Results is null || response.Results.Count == 0)
                    return new List<City>();

                return response.Results.Select(r => new City
                {
                    Id = r.Id,
                    Name = r.Name,
                    Country = r.Country,
                    CountryCode = r.Country_Code,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                }).ToList();
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error: {ex.Message}");
                return new List<City>();
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parse error: {ex.Message}");
                return new List<City>();
            }
        }

        /// <summary> Fetches the current weather and a 7-day forecast for the given coordinates. </summary>
        /// <param name="latitude">Latitude of the location.</param>
        /// <param name="longitude">Longitude of the location.</param>
        /// <returns>Tuple containing the current weather and daily forecast list, or null on failure.</returns>
        public async Task<(CurrentWeather? Current, List<DailyForecast>? Forecast)> GetForecastAsync(double latitude, double longitude, TemperatureUnit temperatureUnit)
        {
            try
            {   
                string apiTemperatureUnit = temperatureUnit == TemperatureUnit.Celsius ? "celsius" : "fahrenheit";
                string url = $"https://api.open-meteo.com/v1/forecast" +
                             $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                             $"&temperature_unit={apiTemperatureUnit}" +
                             $"&current=temperature_2m,windspeed_10m,weathercode,is_day" +
                             $"&daily=temperature_2m_max,temperature_2m_min,weathercode,precipitation_sum,windspeed_10m_max" +
                             $"&timezone=auto&forecast_days=7";

                System.Diagnostics.Debug.WriteLine($"Calling URL: {url}");
                string json = await _httpClient.GetStringAsync(url);

                var response = JsonSerializer.Deserialize<ForecastResponse>(json, JsonOptions);

                if (response is null)
                    return (null, null);

                //Map current weather
                CurrentWeather? current = null;
                if (response.Current is not null)
                {
                    current = new CurrentWeather
                    {
                        Temperature = response.Current.Temperature_2m,
                        WindSpeed = response.Current.WindSpeed_10m,
                        WeatherCode = response.Current.WeatherCode,
                        IsDay = response.Current.Is_Day == 1,
                        Description = WeatherCodeHelper.GetDescription(response.Current.WeatherCode)
                    };
                }

                //Map daily forecast
                List<DailyForecast>? forecast = null;
                if (response.Daily is not null)
                {
                    forecast = response.Daily.Time
                        .Select((dateStr, i) => new DailyForecast
                        {
                            Date = DateTime.Parse(dateStr),
                            TempMax = response.Daily.Temperature_2m_Max[i],
                            TempMin = response.Daily.Temperature_2m_Min[i],
                            WeatherCode = response.Daily.WeatherCode[i],
                            PrecipitationSum = response.Daily.Precipitation_Sum[i],
                            WindSpeedMax = response.Daily.Windspeed_10m_Max[i],
                            Description = WeatherCodeHelper.GetDescription(response.Daily.WeatherCode[i])
                        }).ToList();
                }

                return (current, forecast);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error: {ex.Message}");
                return (null, null);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parse error: {ex.Message}");
                return (null, null);
            }
        }
    }
}
