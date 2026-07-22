using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Services

/// <summary>
/// Entity Framework Core database context for the WeatherApp local SQLite database.
/// </summary>

{
    public class AppDbContext : DbContext
    {
        /// <summary> Table of user's saved favorite cities. </summary>
        public DbSet<FavoriteCity> FavoriteCities { get; set; }

        /// <summary> Application settings stores as single record. </summary>
        public DbSet<AppSettings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WeatherApp",
                "weather.db"
                );

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}
