using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Tests;

public class WeatherRecommendationServiceTests
{
    private readonly WeatherRecommendationService _service = new();

    [Fact]
    public void GetRecommendation_WhenThunderstormCodeIsCurrentWeather_ReturnsThunderstormWarning()
    {
        var current = new CurrentWeather
        {
            Temperature = 20,
            WindSpeed = 10,
            WeatherCode = 95
        };

        string recommendation = _service.GetRecommendation(current, today: null, TemperatureUnit.Celsius);

        Assert.Equal("Thunderstorms are possible. Avoid exposed outdoor areas.", recommendation);
    }

    [Fact]
    public void GetRecommendation_WhenDailyPrecipitationIsAtLeastOneMillimeter_ReturnsUmbrellaRecommendation()
    {
        var current = new CurrentWeather
        {
            Temperature = 20,
            WindSpeed = 10,
            WeatherCode = 0
        };

        var today = new DailyForecast
        {
            WeatherCode = 0,
            PrecipitationSum = 1
        };

        string recommendation = _service.GetRecommendation(current, today, TemperatureUnit.Celsius);

        Assert.Equal("Rain is expected today. Taking an umbrella is a good idea.", recommendation);
    }

    [Fact]
    public void GetRecommendation_WhenFahrenheitTemperatureIsHot_ReturnsHotWeatherRecommendation()
    {
        var current = new CurrentWeather
        {
            Temperature = 86,
            WindSpeed = 10,
            WeatherCode = 0
        };

        string recommendation = _service.GetRecommendation(current, today: null, TemperatureUnit.Fahrenheit);

        Assert.Equal("It is hot today. Stay hydrated.", recommendation);
    }

    [Fact]
    public void GetRecommendation_WhenTemperatureIsCold_ReturnsWarmCoatRecommendation()
    {
        var current = new CurrentWeather
        {
            Temperature = 4,
            WindSpeed = 10,
            WeatherCode = 0
        };

        string recommendation = _service.GetRecommendation(current, today: null, TemperatureUnit.Celsius);

        Assert.Equal("It is cold outside. A warm coat is recommended.", recommendation);
    }

    [Fact]
    public void GetRecommendation_WhenWindSpeedIsStrong_ReturnsWindWarning()
    {
        var current = new CurrentWeather
        {
            Temperature = 20,
            WindSpeed = 40,
            WeatherCode = 0
        };

        string recommendation = _service.GetRecommendation(current, today: null, TemperatureUnit.Celsius);

        Assert.Equal("Strong winds are expected. Secure loose items outdoors.", recommendation);
    }

    [Fact]
    public void GetRecommendation_WhenConditionsAreComfortable_ReturnsOutdoorRecommendation()
    {
        var current = new CurrentWeather
        {
            Temperature = 20,
            WindSpeed = 10,
            WeatherCode = 0
        };

        string recommendation = _service.GetRecommendation(current, today: null, TemperatureUnit.Celsius);

        Assert.Equal("Conditions look comfortable for outdoor activities. Enjoy the weather.", recommendation);
    }
}
