using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp.Views;

public partial class SidebarView : UserControl
{
    public SidebarView()
    {
        InitializeComponent();
    }

    private async void SearchResult_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel viewModel &&
            sender is FrameworkElement { DataContext: City city })
        {
            await viewModel.SelectCityAsync(city);
        }
    }

    private async void Favorite_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel ||
            sender is not FrameworkElement { DataContext: FavoriteCity favorite })
        {
            return;
        }

        var city = new City
        {
            Name = favorite.Name,
            Country = favorite.Country,
            CountryCode = favorite.CountryCode,
            Latitude = favorite.Latitude,
            Longitude = favorite.Longitude
        };

        await viewModel.SelectCityAsync(city);
    }

    private async void RemoveFavorite_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel &&
            sender is FrameworkElement { Tag: FavoriteCity favorite })
        {
            await viewModel.RemoveFavoritesAsync(favorite);
        }
    }
}