using System.Net.Http;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services

/// <summary>
/// Handles all communication with the Open-Meteo API.
/// </summary>

{
    public class WeatherService : IWeatherService
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
                throw new WeatherServiceException(
                    "Could not connect to the city search service. Please check your internet connection.", ex);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parse error: {ex.Message}");
                throw new WeatherServiceException(
                    "The city search service returned data in an unexpected format.", ex);
            }
        }

        /// <summary> Fetches the current weather and a 7-day forecast for the given coordinates. </summary>
        /// <param name="latitude">Latitude of the location.</param>
        /// <param name="longitude">Longitude of the location.</param>
        /// <returns>Tuple containing the current weather and daily forecast list, or null on failure.</returns>
        public async Task<(CurrentWeather? Current, List<DailyForecast>? Forecast, List<HourlyForecast>? Hourly)> GetForecastAsync(double latitude, double longitude, TemperatureUnit temperatureUnit)
        {
            try
            {   
                string apiTemperatureUnit = temperatureUnit == TemperatureUnit.Celsius ? "celsius" : "fahrenheit";
                string url = $"https://api.open-meteo.com/v1/forecast" +
                             $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                             $"&temperature_unit={apiTemperatureUnit}" +
                             $"&current=temperature_2m,windspeed_10m,weathercode,is_day" +
                             $"&daily=temperature_2m_max,temperature_2m_min,weathercode,precipitation_sum,windspeed_10m_max" +
                             $"&hourly=temperature_2m,weathercode,precipitation"+
                             $"&timezone=auto&forecast_days=7";

                System.Diagnostics.Debug.WriteLine($"Calling URL: {url}");
                string json = await _httpClient.GetStringAsync(url);

                var response = JsonSerializer.Deserialize<ForecastResponse>(json, JsonOptions);

                if (response is null)
                    return (null, null, null);

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
                    var daily = response.Daily;

                    int forecastCount = new[]
                    {
                        daily.Time.Count,
                        daily.Temperature_2m_Max.Count,
                        daily.Temperature_2m_Min.Count,
                        daily.WeatherCode.Count,
                        daily.Precipitation_Sum.Count,
                        daily.Windspeed_10m_Max.Count
                    }.Min();

                    forecast = new List<DailyForecast>();

                    for (int i = 0; i < forecastCount; i++)
                    {
                        int weatherCode = daily.WeatherCode[i];

                        forecast.Add(new DailyForecast
                        {
                            Date = DateTime.Parse(daily.Time[i]),
                            TempMax = daily.Temperature_2m_Max[i],
                            TempMin = daily.Temperature_2m_Min[i],
                            WeatherCode = weatherCode,
                            PrecipitationSum = daily.Precipitation_Sum[i],
                            WindSpeedMax = daily.Windspeed_10m_Max[i],
                            Description = WeatherCodeHelper.GetDescription(weatherCode),
                            WeatherIcon = WeatherCodeHelper.GetIcon(weatherCode)
                        });
                    }
                }

                // Map hourly forecast
                List<HourlyForecast>? hourlyForecast = null;
                if (response.Hourly is not null)
                {
                    var hourly = response.Hourly;

                    int hourlyCount = new[]
                    {
                        hourly.Time.Count,
                        hourly.Temperature_2m.Count,
                        hourly.WeatherCode.Count,
                        hourly.Precipitation.Count
                    }.Min();

                    hourlyForecast = new List<HourlyForecast>();

                    DateTime currentHour = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day,
                        DateTime.Now.Hour,
                        0,
                        0);

                    for (int i = 0; i < hourlyCount && hourlyForecast.Count < 12; i++)
                    {
                        DateTime forecastTime = DateTime.Parse(hourly.Time[i]);

                        if (forecastTime < currentHour)
                            continue;

                        int weatherCode = hourly.WeatherCode[i];

                        hourlyForecast.Add(new HourlyForecast
                        {
                            Time = forecastTime,
                            Temperature = hourly.Temperature_2m[i],
                            WeatherCode = weatherCode,
                            Precipitation = hourly.Precipitation[i],
                            WeatherIcon = WeatherCodeHelper.GetIcon(weatherCode)
                        });
                    }
                }

                return (current, forecast, hourlyForecast);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error: {ex.Message}");
                throw new WeatherServiceException(
                    "Could not connect to the weather service. Please check your internet connection.", ex);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parse error: {ex.Message}");
                throw new WeatherServiceException(
                    "The weather service returned data in an unexpected format.", ex);
            }
        }
    }
}
