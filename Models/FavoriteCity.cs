namespace WeatherApp.Models
{

    /// <summary>
    /// Represents a user's saved favorite city stored in the local SQLite database.
    /// </summary>

    public class FavoriteCity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
