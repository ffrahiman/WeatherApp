namespace WeatherApp.Models;

/// <summary> Persisted application settings stored in the local database. </summary>

public class AppSettings
{
    public const int SettingsId = 1;

    public int Id { get; set; } = SettingsId;

    public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;

    public string DefaultCityName { get; set; } = "Hof";
    public string DefaultCountry { get; set; } = "Germany";
    public string DefaultCountryCode { get; set; } = "DE";
    public double DefaultLatitude { get; set; } = 50.3167;
    public double DefaultLongitude { get; set; } = 11.9167;

    /// <summary> Returns a new City object from currently stored default city. </summary>
    public City GetDefaultCity()
    {
        return new City
        {
            Name = DefaultCityName,
            Country = DefaultCountry,
            CountryCode = DefaultCountryCode,
            Latitude = DefaultLatitude,
            Longitude = DefaultLongitude
        };
    }

    /// <summary> Change default city. </summary>
    public void SetDefaultCity(City city)
    {
        ArgumentNullException.ThrowIfNull(city);

        DefaultCityName = city.Name;
        DefaultCountry = city.Country;
        DefaultCountryCode = city.CountryCode;
        DefaultLatitude = city.Latitude;
        DefaultLongitude = city.Longitude;
    }

    /// <summary> Restore Hof as default city </summary>
    public void ResetDefaultCity()
    {
        DefaultCityName = "Hof";
        DefaultCountry = "Germany";
        DefaultCountryCode = "DE";
        DefaultLatitude = 50.3167;
        DefaultLongitude = 11.9167;
    }
}
