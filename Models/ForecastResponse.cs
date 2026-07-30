namespace WeatherApp.Models

/// <summary>
/// Raw JSON response from the Open-Meteo Forecast API.
/// </summary>

{
    public class ForecastResponse
    {
        public CurrentWeatherRaw? Current { get; set; }
        public CurrentWeatherUnits? Current_Units { get; set; }
        public DailyRaw? Daily { get; set; }
        public HourlyRaw? Hourly { get; set; }
    }

    /// <summary> Raw current weather data from the API. </summary>
    public class CurrentWeatherRaw
    {
        public double Temperature_2m { get; set; }
        public double WindSpeed_10m { get; set; }
        public int WeatherCode { get; set; }
        public int Is_Day { get; set; }
    }

    /// <summary> Units block - to confirm temperature unit from API. </summary>
    public class CurrentWeatherUnits
    {
        public string Temperature_2m { get; set; } = string.Empty;
    }

    /// <summary> Raw daily forecast data from the API. </summary>
    public class DailyRaw
    {
        public List<string> Time { get; set; } = new();
        public List<double> Temperature_2m_Max { get; set; } = new();
        public List<double> Temperature_2m_Min { get; set; } = new();
        public List<int> WeatherCode { get; set; } = new();
        public List<double> Precipitation_Sum { get; set; } = new();
        public List<double> Windspeed_10m_Max { get; set; } = new();
    }

    /// <summary> Raw hourly forecast data from the API. </summary>
    public class HourlyRaw
    {
        public List<string> Time { get; set; } = new();
        public List<double> Temperature_2m { get; set; } = new();
        public List<int> WeatherCode { get; set; } = new();
        public List<double> Precipitation { get; set; } = new();
    }
}