using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.Serialization;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels

/// <summary>
/// Main ViewModel for city search, weather display and favorites management.
/// </summary>
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly WeatherService _weatherService;
        private readonly DatabaseService _databaseService;

        // Observable Properties

        /// <summary> Search Box Test </summary>
        [ObservableProperty]
        private string _searchQuery = string.Empty;

        /// <summary> Current City </summary>
        [ObservableProperty]
        private City? _selectedCity;

        /// <summary> Current weather for selected City. </summary>
        [ObservableProperty]
        private CurrentWeather? _currentWeather;

        /// <summary> Weather Forecast for selected City. </summary>
        [ObservableProperty]
        private List<DailyForecast> _forecast = new();

        /// <summary> Search Result Cities </summary>
        [ObservableProperty]
        private List<City> _searchResults = new();

        /// <summary> Saved Favorite Cities </summary>
        [ObservableProperty]
        private List<FavoriteCity> _favorites = new();

        /// <summary>Whether an API call is currently in progress. </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary> Error/Status Message shown to the user. </summary>
        [ObservableProperty]
        private string _statusMessage = string.Empty;

        /// <summary> Navigation Sidebar open status. </summary>
        [ObservableProperty]
        private bool _isSidebarOpen;

        // Constructor

        public MainViewModel()
        {
            _weatherService = new WeatherService();
            _databaseService = new DatabaseService();
        }

        // Commands

        /// <summary> Opens or Closes the navigation sidebar. </summary>
        [RelayCommand]
        private void ToggleSidebar()
        {
            IsSidebarOpen = !IsSidebarOpen;
        }

        /// <summary> Closes navigation sidebar. </summary>
        [RelayCommand]
        private void CloseSidebar()
        {
            IsSidebarOpen = false;
        }

        /// <summary> Searches for cities matching current search query </summary>
        [RelayCommand]
        private async Task SearchCitiesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                StatusMessage = "Please enter a city name.";
                return;
            }

            IsLoading = true;
            StatusMessage = string.Empty;

            try
            {
                SearchResults = await _weatherService.SearchCitiesAsync(SearchQuery);
                if (SearchResults.Count == 0)
                    StatusMessage = "No cities found. Try a different name.";
            }
            catch (Exception ex) 
            {
                StatusMessage = $"Search failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary> Loads weather data for the selected city. </summary>
        [RelayCommand]
        public async Task SelectCityAsync(City city)
        {
            if (city is null)
                return;

            SelectedCity = city;
            IsLoading = true;
            StatusMessage = String.Empty;

            try
            {
                var (current, forecast) = await _weatherService.GetForecastAsync(
                    city.Latitude,
                    city.Longitude
                );

                CurrentWeather = current;
                Forecast = forecast ?? new List<DailyForecast>();

                if (current is null)
                    StatusMessage = "Could not load weather data.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load weather: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary> Adds the currently selected city to favorites. </summary>
        [RelayCommand]
        private async Task AddFavoritesAsync()
        {
            if (SelectedCity is null)
                return;

            try
            {
                await _databaseService.AddFavoriteCityAsync(SelectedCity);
                await LoadFavoritesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not save favorite: {ex.Message}";
            }
        }

        /// <summary> Removes a city from favorites. </summary>
        [RelayCommand]
        public async Task RemoveFavoritesAsync(FavoriteCity city)
        {
            if (city is null)
                return;

            try
            {
                await _databaseService.RemoveFavoriteCityAsync(city);
                await LoadFavoritesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not remove favorite: {ex.Message}";
            }
        }

        /// <summary> Loadsd all favorites from the database into the UI. </summary>
        [RelayCommand]
        public async Task LoadFavoritesAsync()
        {
            try
            {
                Favorites = await _databaseService.GetFavoriteCitiesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not load favorites: {ex.Message}";
            }
        }
    }
}   
