using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Services

/// <summary>
/// Handles all database operations for favorite cities using Entity Framework Core.
/// </summary>

{
    public class DatabaseService
    {
        /// <summary> Retrieves all saved favortie cities from the database. </summary>
        /// <returns>A list of favorite cities.</returns>
        public async Task<List<FavoriteCity>> GetFavoriteCitiesAsync()
        {
            using var db = new AppDbContext();
            return await db.FavoriteCities.ToListAsync();
        }

        /// <summary> Saves a new favorite city to the database. </summary>
        /// <param name="city">The city to save as a favorite.</param>
        public async Task AddFavoriteCityAsync(City city)
        {
            using var db = new AppDbContext();

            bool alreadyexists = await db.FavoriteCities
                .AnyAsync(f => f.Name == city.Name && f.Country == city.Country);

            if (alreadyexists)
            {
                return;
            }

            var favorite = new FavoriteCity
            {
                Name        = city.Name,
                Country     = city.Country,
                CountryCode = city.CountryCode,
                Latitude    = city.Latitude,
                Longitude   = city.Longitude
            };

            db.FavoriteCities.Add(favorite);
            await db.SaveChangesAsync();
        }

        /// <summary> Removes a favorite city from the database by name and country. </summary>
        /// <param name="city">The city to remove from favorites.</param>
        public async Task RemoveFavoriteCityAsync(FavoriteCity city)
        {
            using var db = new AppDbContext();

            var existing = await db.FavoriteCities
                .FirstOrDefaultAsync(f => f.Name == city.Name && f.Country == city.Country);

            if (existing is null)
                return;
            
            db.FavoriteCities.Remove(existing);
            await db.SaveChangesAsync();
        }
    }
}
