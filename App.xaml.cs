using System.Configuration;
using System.Data;
using System.Windows;
using WeatherApp.Services;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var service = new WeatherService();

            using var db = new AppDbContext();
            await db.Database.EnsureCreatedAsync();

            // Service Test
            //var cities = await service.SearchCitiesAsync("Berlin");
            //foreach (var city in cities)
            //    System.Diagnostics.Debug.WriteLine($"Found: {city.DisplayName} ({city.Latitude}, {city.Longitude})");

            // Weather code and forecast test
            //var (current, forecast) = await service.GetForecastAsync(52.52, 13.41);
            //if (current is not null)
            //    System.Diagnostics.Debug.WriteLine($"Now: {current.Description}, {current.Temperature}°C, Wind: {current.WindSpeed} km/h");
            //if (forecast is not null)
            //    foreach (var day in forecast)
            //        System.Diagnostics.Debug.WriteLine($"{day.DayName}: {day.Description}, {day.TempMin}° - {day.TempMax}°C");
        }
    }

}
