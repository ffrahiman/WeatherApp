using System;
using System.Collections.Generic;
using System.Text;
using WeatherApp.Models;

namespace WeatherApp.Services

/// <summary>
/// Maps Open-Meteo WMO weather codes to a human-readable description.
/// </summary>

{
    public static class WeatherCodeHelper
    {
        /// <summary> Returns a short description for a given weather code. </summary>
        public static string GetDescription(int code) => code switch
        {
            0               => "Clear sky",
            1               => "Mostly clear",
            2               => "Partly cloudy",
            3               => "Overcast",
            45 or 48        => "Foggy",
            51 or 53        => "Light Drizzle",
            55              => "Dense Drizzle",
            56 or 57        => "Freezing Drizzle",
            61 or 63        => "Slight Rain",
            65              => "Heavy Rain",
            71 or 73        => "Light Snow",
            75              => "Heavy Snow",
            77              => "Snow grains",
            80 or 81        => "Slight showers",
            82              => "Heavy showers",
            85 or 86        => "Snow showers",
            95              => "Thunderstorm",
            96 or 99        => "Thunderstorm with hail",
            _               => "Unknown"
        };

        /// <summary>
        /// Returns the visual theme for a weather code and time of day.
        /// </summary>
        public static WeatherTheme GetTheme(int code, bool isDay)
        {
            if (code is 0 or 1)
            {
                return isDay ? WeatherTheme.ClearDay : WeatherTheme.ClearNight;
            }

            if (code is 2 or 3)
            {
                return isDay ? WeatherTheme.CloudyDay : WeatherTheme.CloudyNight;
            }

            if (code is 45 or 48)
            {
                return isDay ? WeatherTheme.FogDay : WeatherTheme.FogNight;
            }

            if (code is 51 or 53 or 55 or 56 or 57 or 61 or 63 or 65 or 66 or 67 or 80 or 81 or 82)
            {
                return isDay ? WeatherTheme.RainDay : WeatherTheme.RainNight;
            }

            if (code is 71 or 73 or 75 or 77 or 85 or 86)
            {
                return isDay ? WeatherTheme.SnowDay : WeatherTheme.SnowNight;
            }

            if (code is 95 or 96 or 99)
            {
                return isDay ? WeatherTheme.StormDay : WeatherTheme.StormNight;
            }

            return isDay ? WeatherTheme.CloudyDay : WeatherTheme.CloudyNight;
        }
    }

}
