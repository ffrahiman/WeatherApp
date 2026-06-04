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

            // Service Test
            var service = new WeatherService();
            var cities = await service.SearchCitiesAsync("Berlin");
            foreach (var city in cities)
                System.Diagnostics.Debug.WriteLine($"Found: {city.DisplayName} ({city.Latitude}, {city.Longitude})");
        }
    }

}
