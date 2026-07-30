# WeatherApp

WeatherApp ist eine WPF-Desktopanwendung fuer aktuelle Wetterdaten, Vorhersagen und persoenliche Favoriten.

## Funktionen

- Suche nach Staedten ueber die Open-Meteo Geocoding API
- Anzeige des aktuellen Wetters
- 12-Stunden-Vorhersage und 7-Tage-Vorhersage
- Wetterempfehlungen, zum Beispiel bei Regen, Sturm, Schnee, Hitze oder Kaelte
- Favoritenverwaltung mit lokaler Speicherung in SQLite
- Einstellungen fuer Temperatureinheit und Standardstadt
- Automatische Datenbankmigrationen mit Entity Framework Core

## Technischer Stack

- C# / WPF
- .NET 10 Windows
- MVVM mit CommunityToolkit.Mvvm
- Entity Framework Core mit SQLite
- Microsoft.Extensions.DependencyInjection
- Open-Meteo API
- xUnit fuer Unit Tests

## Ausfuehren

Voraussetzung ist das passende .NET SDK/Runtime fuer `net10.0-windows`.

```powershell
dotnet build
dotnet run
```

## Tests

```powershell
dotnet test
```

Die Tests pruefen vor allem Wettercode-Mapping und Empfehlungslogik.
