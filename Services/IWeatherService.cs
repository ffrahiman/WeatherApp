using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherService
{
    Task<List<City>> SearchCitiesAsync(string cityName);

    Task<(CurrentWeather? Current, List<DailyForecast>? Forecast, List<HourlyForecast>? Hourly)> GetForecastAsync(
        double latitude,
        double logitude,
        TemperatureUnit temperatureUnit);
}