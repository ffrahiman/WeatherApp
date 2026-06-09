using System.Configuration;
using System.Data;
using System.Windows;
using WeatherApp.Services;
using WeatherApp.Models;

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

            // Database Favorites Test
            var dbService = new DatabaseService();

            var cities = await service.SearchCitiesAsync("Munich");
            var munich = cities.FirstOrDefault();

            if (munich != null)
            {
                System.Diagnostics.Debug.WriteLine($"Found: {munich.DisplayName}");

                await dbService.AddFavoriteCityAsync(munich);
                System.Diagnostics.Debug.WriteLine($"Added {munich.DisplayName} to favorites.");

                var favorites = await dbService.GetFavoriteCitiesAsync();
                System.Diagnostics.Debug.WriteLine($"Total favorites: {favorites.Count}");
                foreach ( var f in favorites )
                    System.Diagnostics.Debug.WriteLine($" - Favorite: {f.Name}, {f.Country}");

                await dbService.RemoveFavoriteCityAsync(favorites.First(f => f.Name == "Berlin"));
                System.Diagnostics.Debug.WriteLine($"Removed Berlin from favorites.");

                var afterRemove = await dbService.GetFavoriteCitiesAsync();
                System.Diagnostics.Debug.WriteLine($"Total favorites after removal: {afterRemove.Count}");
                foreach (var f in afterRemove)
                    System.Diagnostics.Debug.WriteLine($" - Favorite: {f.Name}, {f.Country}");
            }

            //await dbService.AddFavoriteCityAsync(new City
            //{
            //    Name = "Berlin",
            //    Country = "Germany",
            //    CountryCode = "DE",
            //    Latitude = 52.52,
            //    Longitude = 13.41
            //});

            //var favorites = await dbService.GetFavoriteCitiesAsync();
            //foreach (var fav in favorites)
            //    System.Diagnostics.Debug.WriteLine($"Favorite: {fav.Name}, {fav.Country}");
        }
    }

}
