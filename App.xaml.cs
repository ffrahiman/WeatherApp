using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Services;
using WeatherApp.ViewModels;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var services = new ServiceCollection();

                services.AddSingleton<IWeatherService, WeatherService>();
                services.AddSingleton<IDatabaseService, DatabaseService>();
                services.AddSingleton<IWeatherRecommendationService, WeatherRecommendationService>();

                services.AddSingleton<MainViewModel>();
                services.AddTransient<MainWindow>();

                _serviceProvider = services.BuildServiceProvider();

                await using var db = new AppDbContext();
                await db.Database.MigrateAsync();

                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                await mainViewModel.InitializeAsync();

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
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