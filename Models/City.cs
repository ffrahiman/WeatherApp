using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Models

/// <summary>
/// Represents a city returned from the Open-Meteo Geocoding API or stores as a user favourite.
/// </summary>

{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsFavorite { get; set; }

        public string DisplayName => $"{Name}, {Country}";
    }
}
