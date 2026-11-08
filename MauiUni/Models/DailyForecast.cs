namespace MauiUni.Models;

public class DailyForecast
{
    public string Data { get; set; } = string.Empty;
    public string TemperaturaMax { get; set; } = string.Empty;
    public string TemperaturaMin { get; set; } = string.Empty;
    public string DescriereStare { get; set; } = string.Empty;
    public string Iconita { get; set; } = "🌤️";
}