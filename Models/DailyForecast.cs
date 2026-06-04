using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace WeatherApp.Models

/// <summary>
/// Represents weather forecast data for a single day. 
/// </summary>

{
    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public double TempMax { get; set; }
        public double TempMin { get; set; }
        public int WeatherCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public string WeatherIcon { get; set; } = string.Empty;
        public double PrecipitationSum { get; set; }
        public double WindSpeedMax { get; set; }

        /// <summary> Full day name (Monday) </summary>
        public string DayName => Date.ToString("dddd");

        /// <summary> Short date String (01. Jan) </summary>
        public string ShortDate => Date.ToString("dd. MMM");
    }
}
