using System.Windows;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Services;
using WeatherApp.ViewModels;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                await using var db = new AppDbContext();
                await db.Database.MigrateAsync();

                IWeatherService weatherService = new WeatherService();
                IDatabaseService databaseService = new DatabaseService();
                IWeatherRecommendationService recommendationService = new WeatherRecommendationService();

                var mainViewModel = new MainViewModel(weatherService, databaseService, recommendationService);
                await mainViewModel.InitializeAsync();

                var mainWindow = new MainWindow(mainViewModel);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"The application could not start correctly:\n\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}