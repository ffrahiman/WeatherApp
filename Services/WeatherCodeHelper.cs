using System;
using System.Collections.Generic;
using System.Text;

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
            1               => "Mainly clear",
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
    }
}
