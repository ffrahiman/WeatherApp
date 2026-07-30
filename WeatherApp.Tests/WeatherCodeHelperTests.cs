using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Tests;

public class WeatherCodeHelperTests
{
    [Theory]
    [InlineData(0, "Clear sky")]
    [InlineData(61, "Slight Rain")]
    [InlineData(95, "Thunderstorm")]
    [InlineData(-1, "Unknown")]
    public void GetDescription_ReturnsExpectedDescriptionForWeatherCode(int code, string expectedDescription)
    {
        string description = WeatherCodeHelper.GetDescription(code);

        Assert.Equal(expectedDescription, description);
    }

    [Theory]
    [InlineData(0, true, WeatherTheme.ClearDay)]
    [InlineData(0, false, WeatherTheme.ClearNight)]
    [InlineData(61, true, WeatherTheme.RainDay)]
    [InlineData(61, false, WeatherTheme.RainNight)]
    [InlineData(95, true, WeatherTheme.StormDay)]
    [InlineData(95, false, WeatherTheme.StormNight)]
    public void GetTheme_ReturnsExpectedThemeForWeatherCodeAndDaylight(int code, bool isDay, WeatherTheme expectedTheme)
    {
        WeatherTheme theme = WeatherCodeHelper.GetTheme(code, isDay);

        Assert.Equal(expectedTheme, theme);
    }

    [Theory]
    [InlineData(0, "☀️")]
    [InlineData(1, "🌤️")]
    [InlineData(2, "⛅")]
    [InlineData(3, "☁")]
    [InlineData(45, "≋")]
    [InlineData(61, "🌧️")]
    [InlineData(71, "🌨️")]
    [InlineData(75, "❄️")]
    [InlineData(95, "⛈️")]
    [InlineData(-1, " ")]
    public void GetIcon_ReturnsExpectedIconForWeatherCode(int code, string expectedIcon)
    {
        string icon = WeatherCodeHelper.GetIcon(code);

        Assert.Equal(expectedIcon, icon);
    }
}
