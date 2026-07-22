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

            var mainViewModel = new MainViewModel();
            await mainViewModel.InitializeAsync();

            var mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();
        }
    }

}
