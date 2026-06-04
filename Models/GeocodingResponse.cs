using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Models

/// <summary>
/// Represents the raw JSON response from the Open-Meteo Geocoding API.
/// </summary>

{
    public class GeocodingResponse
    {
        public List<GeocodingResult> Results { get; set; } = new();
    }

    /// <summary> A single city result from the Geocoding API. </summary>
    public class GeocodingResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Country_Code { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
