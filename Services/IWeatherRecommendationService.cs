using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherRecommendationService
{
    string GetRecommendation(
        CurrentWeather current,
        DailyForecast? today,
        TemperatureUnit temperatureUnit);
}