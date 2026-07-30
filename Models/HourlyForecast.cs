namespace WeatherApp.Models;

/// <summary>
/// Represents weather forecast data for a single hour.
/// </summary>
public class HourlyForecast
{
    public DateTime Time { get; set; }
    public double Temperature { get; set; }
    public double Precipitation { get; set; }
    public int WeatherCode { get; set; }
    public string WeatherIcon { get; set; } = string.Empty;

    public string DisplayTime => Time.ToString("HH:mm");
}