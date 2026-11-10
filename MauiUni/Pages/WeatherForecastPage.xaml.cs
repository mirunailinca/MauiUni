using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class WeatherForecastPage : ContentPage
{
    private readonly HttpClient _httpClient = new();

    public WeatherForecastPage()
    {
        InitializeComponent();
    }

    private async void OnCautareVremeaClicked(object sender, EventArgs e)
    {
        string oras = CityEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(oras))
        {
            await DisplayAlert("Eroare", "Te rog introdu numele unui oraș.", "OK");
            return;
        }

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        SearchButton.IsEnabled = false;

        try
        {
            string geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(oras)}&count=1&language=ro&format=json";
            var geoResponse = await _httpClient.GetFromJsonAsync<GeoResponse>(geoUrl);

            if (geoResponse?.Results == null || geoResponse.Results.Count == 0)
            {
                await DisplayAlert("Negăsit", "Nu s-a găsit orașul introdus. Verifică ortografia.", "OK");
                return;
            }

            var locatie = geoResponse.Results[0];
            double lat = locatie.Latitude;
            double lon = locatie.Longitude;

            string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,weather_code&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=auto";
            var weatherResponse = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(weatherUrl);

            if (weatherResponse != null)
            {
                double tempCurenta = weatherResponse.Current.Temperature2m;
                int codMeteo = weatherResponse.Current.WeatherCode;
                WeatherLabel.Text = $"{tempCurenta:F1} °C, {ObtineDescriereStare(codMeteo)} ({locatie.Name})";
                WeatherIconLabel.Text = ObtineIconita(codMeteo);

                var listaPrognoza = new List<MauiUni.Models.DailyForecast>();
                int zileDeAfisat = Math.Min(7, weatherResponse.Daily.Time.Count);

                for (int i = 0; i < zileDeAfisat; i++)
                {
                    DateTime data = DateTime.Parse(weatherResponse.Daily.Time[i]);
                    int codZi = weatherResponse.Daily.WeatherCode[i];

                    listaPrognoza.Add(new MauiUni.Models.DailyForecast
                    {
                        Data = data.ToString("dddd, dd MMM", new System.Globalization.CultureInfo("ro-RO")),
                        TemperaturaMax = $"{weatherResponse.Daily.Temperature2mMax[i]:F0}°C",
                        TemperaturaMin = $"{weatherResponse.Daily.Temperature2mMin[i]:F0}°C",
                        DescriereStare = ObtineDescriereStare(codZi),
                        Iconita = ObtineIconita(codZi)
                    });
                }

                ForecastCollectionView.ItemsSource = listaPrognoza;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare", $"A apărut o problemă la preluarea vremii: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            SearchButton.IsEnabled = true;
        }
    }

    private string ObtineDescriereStare(int cod)
    {
        return cod switch
        {
            0 => "Senin",
            1 or 2 or 3 => "Parțial înnorat",
            45 or 48 => "Ceață",
            51 or 53 or 55 or 56 or 57 => "Burniță",
            61 or 63 or 65 or 66 or 67 => "Ploaie",
            71 or 73 or 75 or 77 => "Ninsoare",
            80 or 81 or 82 => "Ploaie intensă",
            95 or 96 or 99 => "Furtună",
            _ => "Schimbător"
        };
    }

    private string ObtineIconita(int cod)
    {
        return cod switch
        {
            0 => "☀️",
            1 or 2 or 3 => "⛅",
            45 or 48 => "🌫️",
            51 or 53 or 55 or 61 or 63 => "🌧️",
            71 or 73 or 75 => "❄️",
            95 or 96 or 99 => "⛈️",
            _ => "🌤️"
        };
    }
}