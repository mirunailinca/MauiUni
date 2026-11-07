using System.Text.Json.Serialization;

namespace MauiUni.Models;

public class Attraction
{
    [JsonPropertyName("attraction")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attractionLabel")]
    public string Nume { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Descriere { get; set; } = string.Empty;

    [JsonPropertyName("cityLabel")]
    public string Oras { get; set; } = string.Empty;

    [JsonPropertyName("countryLabel")]
    public string Tara { get; set; } = string.Empty;

    public string TipSiOras => string.IsNullOrWhiteSpace(Tara) ? Oras : $"{Oras} · {Tara}";

    public string DescriereAfisare => string.IsNullOrWhiteSpace(Descriere)
        ? "Fără descriere."
        : Descriere;
}
