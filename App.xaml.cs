using System.Windows;
using WeatherApp.Services;
using WeatherApp.Models;
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
            using var db = new AppDbContext();
            await db.Database.EnsureCreatedAsync();

            IWeatherService weatherService = new WeatherService();
            IDatabaseService databaseService = new DatabaseService();
            IWeatherRecommendationService recommendationService = new WeatherRecommendationService();

            var mainViewModel = new MainViewModel(weatherService, databaseService, recommendationService);
            await mainViewModel.InitializeAsync();

            var mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();
        }
    }
}