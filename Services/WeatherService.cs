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
    }
}
