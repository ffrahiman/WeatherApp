using System;
using System.Collections.Generic;
using System.Text;
using WeatherApp.Models;

namespace WeatherApp.Services;

/// <summary>
/// Provides practical weather recommendations based on the current weather conditions.
/// </summary>

public class WeatherRecommendationService : IWeatherRecommendationService
{

    /// <summary> Returns the highest-priority recommendation for the current weather. </summary>
    public string GetRecommendation(
        CurrentWeather current,
        DailyForecast? today,
        TemperatureUnit temperatureUnit)
    {
        ArgumentNullException.ThrowIfNull(current);

        int currentCode = current.WeatherCode;
        int forecastCode = today?.WeatherCode ?? currentCode;

        double temperatureCelsius = temperatureUnit == TemperatureUnit.Fahrenheit ? (current.Temperature - 32) * 5 / 9 : current.Temperature;

        if (currentCode is 95 or 96 or 99 || forecastCode is 95 or 96 or 99)
        {
            return "Thunderstorms are possible. Avoid exposed outdoor areas.";
        }

        if (currentCode is 56 or 57 or 66 or 67 || forecastCode is 56 or 57 or 66 or 67)
        {
            return "Freezing precipitation is possible. Watch for icy surfaces.";
        }

        if (currentCode is 71 or 73 or 75 or 77 or 85 or 86 || forecastCode is 71 or 73 or 75 or 77 or 85 or 86)
        {
            return "Snow is expected. Dress warmly and allow extra travel time.";
        }

        if (currentCode is 51 or 53 or 55 or 61 or 63 or 65 or 80 or 81 or 82 || forecastCode is 51 or 53 or 55 or 61 or 63 or 65 or 80 or 81 or 82 || today?.PrecipitationSum >= 1)
        {
            return "Rain is expected today. Taking an umbrella is a good idea.";
        }

        if (currentCode is 45 or 48 || forecastCode is 45 or 48)
        {
            return "Visibility may be reduced by fog. Take care while travelling.";
        }

        if (current.WindSpeed >= 40)
        {
            return "Strong winds are expected. Secure loose items outdoors.";
        }

        if (temperatureCelsius <= 5)
        {
            return "It is cold outside. A warm coat is recommended.";
        }

        if (temperatureCelsius >= 28)
        {
            return "It is hot today. Stay hydrated.";
        }

        return "Conditions look comfortable for outdoor activities. Enjoy the weather.";
    }
}