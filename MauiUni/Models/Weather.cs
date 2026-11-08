using System.Text.Json.Serialization;

namespace MauiUni.Models;

public class GeoResponse
{
    [JsonPropertyName("results")]
    public List<GeoLocation> Results { get; set; } = new();
}

public class GeoLocation
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class WeatherApiResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather Current { get; set; } = new();

    [JsonPropertyName("daily")]
    public DailyWeather Daily { get; set; } = new();
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}

public class DailyWeather
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double> Temperature2mMax { get; set; } = new();

    [JsonPropertyName("temperature_2m_min")]
    public List<double> Temperature2mMin { get; set; } = new();
}