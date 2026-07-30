using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IDatabaseService
{
    Task<List<FavoriteCity>> GetFavoriteCitiesAsync();

    Task AddFavoriteCityAsync(City city);

    Task RemoveFavoriteCityAsync(FavoriteCity city);

    Task<AppSettings> GetSettingsAsync();

    Task SaveSettingsAsync(AppSettings settings);
}