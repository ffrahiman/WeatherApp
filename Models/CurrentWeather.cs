namespace WeatherApp.Models

/// <summary>
/// Represents the current weather conditions for a location.
/// </summary>

{
    public class CurrentWeather
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WeatherCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public string WeatherIcon { get; set; } = string.Empty;
        public bool IsDay { get; set; }
    }
}
