using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels

/// <summary>
/// Main ViewModel for city search, weather display and favorites management.
/// </summary>
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IWeatherService _weatherService;
        private readonly IDatabaseService _databaseService;
        private readonly IWeatherRecommendationService _recommendationService;

        // Observable Properties

        /// <summary> Visual theme based on weather and time of day. </summary>
        [ObservableProperty]
        private WeatherTheme _weatherTheme = WeatherTheme.ClearDay;

        /// <summary> Search Box Test </summary>
        [ObservableProperty]
        private string _searchQuery = string.Empty;

        /// <summary> Current City </summary>
        [ObservableProperty]
        private City? _selectedCity;

        /// <summary> Current weather for selected City. </summary>
        [ObservableProperty]
        private CurrentWeather? _currentWeather;

        /// <summary> Today's forecast, used for high/low and detail boxes. </summary>
        [ObservableProperty]
        private DailyForecast? _todayForecast;

        /// <summary> Hourly forecast for the next 12 hours. </summary>
        [ObservableProperty]
        private List<HourlyForecast> _hourlyForecast = new();

        /// <summary> Weather Forecast for selected City. </summary>
        [ObservableProperty]
        private List<DailyForecast> _forecast = new();

        /// <summary> Recommendation based on the weather. </summary>
        [ObservableProperty]
        private string _recommendationText = string.Empty;

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

        /// <summary> Settings panel open status. </summary>
        [ObservableProperty]
        private bool _isSettingsOpen;

        /// <summary>Persistent application settings.</summary>
        [ObservableProperty]
        private AppSettings _settings = new();

        public string TemperatureUnitDisplay => Settings.TemperatureUnit == TemperatureUnit.Celsius ? "Celsius (°C)" : "Fahrenheit (°F)";

        public string DefaultCityDisplayName => $"{Settings.DefaultCityName}, {Settings.DefaultCountry}";

        partial void OnSettingsChanged(AppSettings value)
        {
            OnPropertyChanged(nameof(TemperatureUnitDisplay));
            OnPropertyChanged(nameof(DefaultCityDisplayName));
        }

        /// <summary> Whether the selected city is saved as a favorite. </summary>
        [ObservableProperty]
        private bool _isSelectedCityFavorite;

        public string FavoriteButtonText => IsSelectedCityFavorite ? "★" : "☆";

        public string FavoriteButtonToolTip => IsSelectedCityFavorite ? "Remove from Favorites" : "Add to Favorites";

        partial void OnIsSelectedCityFavoriteChanged(bool value)
        {
            OnPropertyChanged(nameof(FavoriteButtonText));
            OnPropertyChanged(nameof(FavoriteButtonToolTip));
        }


        // Constructor

        public MainViewModel(
            IWeatherService weatherService,
            IDatabaseService databaseService,
            IWeatherRecommendationService recommendationService)
        {
            _weatherService = weatherService;
            _databaseService = databaseService;
            _recommendationService = recommendationService;
        }

        /// <summary> Loads persisted data and displays weather for the default city. </summary>
        public async Task InitializeAsync()
        {
            try
            {
                Settings = await _databaseService.GetSettingsAsync();
            }
            catch (Exception ex)
            {
                Settings = new AppSettings();
                StatusMessage = $"Could not load settings: {ex.Message}";
            }

            await LoadFavoritesAsync();
            await SelectCityAsync(Settings.GetDefaultCity());
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
            catch (WeatherServiceException ex)
            {
                StatusMessage = ex.Message;
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
            UpdateSelectedCityFavoriteState();
            IsLoading = true;
            StatusMessage = String.Empty;

            try
            {
                var (current, forecast, hourly) = await _weatherService.GetForecastAsync(
                    city.Latitude,
                    city.Longitude,
                    Settings.TemperatureUnit
                );

                CurrentWeather = current;
                Forecast = forecast ?? new List<DailyForecast>();
                TodayForecast = Forecast.FirstOrDefault();
                HourlyForecast = hourly ?? new List<HourlyForecast>();
                UpdateSelectedCityFavoriteState();

                if (current is not null)
                {
                    WeatherTheme = WeatherCodeHelper.GetTheme(current.WeatherCode, current.IsDay);
                }

                RecommendationText = current is null
                    ? string.Empty
                    : _recommendationService.GetRecommendation(current, Forecast.FirstOrDefault(),
                        Settings.TemperatureUnit);

                if (current is null)
                    StatusMessage = "Could not load weather data.";
            }
            catch (WeatherServiceException ex)
            {
                StatusMessage = ex.Message;
                RecommendationText = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load weather: {ex.Message}";
                RecommendationText = string.Empty;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary> Loads weather for a saved favorite city. </summary>
        /// <param name="city">The favorite city for which to load weather.</param>
        [RelayCommand]
        public async Task SelectFavoriteAsync(FavoriteCity favorite)
        {
            if (favorite is null)
                return;

            var city = new City
            {
                Name = favorite.Name,
                Country = favorite.Country,
                CountryCode = favorite.CountryCode,
                Latitude = favorite.Latitude,
                Longitude = favorite.Longitude
            };

            await SelectCityAsync(city);
        }

        /// <summary> Adds the currently selected city to favorites. </summary>
        [RelayCommand]
        private async Task ToggleFavoriteAsync()
        {
            if (SelectedCity is null)
                return;

            try
            {
                var existingFavorite = FindFavorite(SelectedCity);

                if (existingFavorite is null)
                {
                    await _databaseService.AddFavoriteCityAsync(SelectedCity);
                    StatusMessage = $"{SelectedCity.Name} added to favorites.";
                }
                else
                {
                    await _databaseService.RemoveFavoriteCityAsync(existingFavorite);
                    StatusMessage = $"{SelectedCity.Name} removed from favorites.";
                }

                await LoadFavoritesAsync();
                UpdateSelectedCityFavoriteState();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not update favorite: {ex.Message}";
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
                UpdateSelectedCityFavoriteState();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not remove favorite: {ex.Message}";
            }
        }

        /// <summary> Loads all favorites from the database into the UI. </summary>
        [RelayCommand]
        public async Task LoadFavoritesAsync()
        {
            try
            {
                Favorites = await _databaseService.GetFavoriteCitiesAsync();
                UpdateSelectedCityFavoriteState();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not load favorites: {ex.Message}";
            }
        }

        /// <summary> Open, Close Settings </summary>
        [RelayCommand]
        private void OpenSettings()
        {
            IsSidebarOpen = false;
            IsSettingsOpen = true;
        }
        [RelayCommand]
        private void CloseSettings()
        {
            IsSettingsOpen = false;
        }

        /// <summary> Temperature Commands </summary>
        [RelayCommand]
        private async Task UseCelsiusAsync()
        {
            await ChangeTemperatureUnitAsync(TemperatureUnit.Celsius);
        }
        [RelayCommand]
        private async Task UseFahrenheitAsync()
        {
            await ChangeTemperatureUnitAsync(TemperatureUnit.Fahrenheit);
        }

        private async Task ChangeTemperatureUnitAsync(TemperatureUnit unit)
        {
            if (Settings.TemperatureUnit == unit)
                return;

            var previousUnit = Settings.TemperatureUnit;
            Settings.TemperatureUnit = unit;

            try
            {
                await _databaseService.SaveSettingsAsync(Settings);
                OnPropertyChanged(nameof(TemperatureUnitDisplay));

                if (SelectedCity is not null)
                {
                    await SelectCityAsync(SelectedCity);
                }
            }
            catch (Exception ex)
            {
                Settings.TemperatureUnit = previousUnit;
                OnPropertyChanged(nameof(TemperatureUnitDisplay));
                StatusMessage = $"Could not save temperature unit: {ex.Message}";
            }
        }

        /// <summary> Default City Commands </summary>
        [RelayCommand]
        private async Task UseCurrentCityAsDefaultAsync()
        {
            if (SelectedCity is null)
            {
                StatusMessage = "Select a city before setting the default.";
                return;
            }

            var previousCity = Settings.GetDefaultCity();
            Settings.SetDefaultCity(SelectedCity);

            try
            {
                await _databaseService.SaveSettingsAsync(Settings);
                OnPropertyChanged(nameof(DefaultCityDisplayName));
                StatusMessage = $"Default city set to {SelectedCity.Name}, {SelectedCity.Country}.";
            }
            catch (Exception ex)
            {
                Settings.SetDefaultCity(previousCity);
                OnPropertyChanged(nameof(DefaultCityDisplayName));
                StatusMessage = $"Could not save default city: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ResetDefaultCityAsync()
        {
            var previousCity = Settings.GetDefaultCity();
            Settings.ResetDefaultCity();

            try
            {
                await _databaseService.SaveSettingsAsync(Settings);
                OnPropertyChanged(nameof(DefaultCityDisplayName));
                StatusMessage = $"Default city reset to {Settings.DefaultCityName}, {Settings.DefaultCountry}.";
            }
            catch (Exception ex)
            {
                Settings.SetDefaultCity(previousCity);
                OnPropertyChanged(nameof(DefaultCityDisplayName));
                StatusMessage = $"Could not reset default city: {ex.Message}";
            }
        }

        private void UpdateSelectedCityFavoriteState()
        {
            IsSelectedCityFavorite = SelectedCity is not null &&
                                     Favorites.Any(f => IsSameCity(SelectedCity, f));
        }

        private static bool IsSameCity(City city, FavoriteCity favorite)
        {
            return city.Name == favorite.Name &&
                   city.Country == favorite.Country;
        }

        private FavoriteCity? FindFavorite(City city)
        {
            return Favorites.FirstOrDefault(f => IsSameCity(city, f));
        }
    }
}