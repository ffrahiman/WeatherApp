using System.Windows;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private async void SearchResult_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: City city })
            await _viewModel.SelectCityAsync(city);
    }

    private async void Favorite_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FavoriteCity fav })
        {
            var city = new City
            {
                Name = fav.Name,
                Country = fav.Country,
                CountryCode = fav.CountryCode,
                Latitude = fav.Latitude,
                Longitude = fav.Longitude,
            };
            await _viewModel.SelectCityAsync(city);
        }
    }

    private async void RemoveFavorite_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: FavoriteCity fav })
            await _viewModel.RemoveFavoritesAsync(fav);
    }
}