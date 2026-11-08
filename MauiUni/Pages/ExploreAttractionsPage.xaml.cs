using System.Globalization;
using System.Text;
using System.Text.Json;
using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class ExploreAttractionsPage : ContentPage
{
    List<Attraction> _toate = new();

    public ExploreAttractionsPage()
    {
        InitializeComponent();
    }

    private async void OnCautareOras(object sender, EventArgs e)
    {
        if (_toate.Count == 0)
            await IncarcaAtractii();

        AplicaFiltre();
    }

    private async Task IncarcaAtractii()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("json_atractii_turistice.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        var dinJson = JsonSerializer.Deserialize<List<Attraction>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<Attraction>();

        _toate = dinJson
            .Where(a => !string.IsNullOrWhiteSpace(a.Nume) && !string.IsNullOrWhiteSpace(a.Oras))
            .GroupBy(a => $"{a.Id}|{a.Oras}")
            .Select(g => g.First())
            .ToList();
    }

    private void AplicaFiltre()
    {
        string orasCautat = FaraDiacritice(BaraCautare.Text ?? "");
        if (string.IsNullOrWhiteSpace(orasCautat))
        {
            ListaAtractiiView.ItemsSource = null;
            MesajLabel.IsVisible = true;
            MesajLabel.Text = "Scrie un oraș, apoi apasă căutarea.";
            return;
        }

        var lista = _toate.Where(a => FaraDiacritice(a.Oras).Contains(orasCautat)).ToList();
        ListaAtractiiView.ItemsSource = lista;

        if (lista.Count == 0)
        {
            MesajLabel.IsVisible = true;
            MesajLabel.Text = $"Nu am găsit obiective în „{BaraCautare.Text?.Trim()}”.";
        }
        else
        {
            MesajLabel.IsVisible = true;
            MesajLabel.Text = lista.Count == 1
                ? $"Un obiectiv în {lista[0].Oras}"
                : $"{lista.Count} obiective în {lista[0].Oras}";
        }
    }

    private static string FaraDiacritice(string text)
    {
        var normalizat = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizat.Length);
        foreach (var c in normalizat)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString();
    }

    private async void OnAdaugaLaItinerariuClicked(object sender, EventArgs e)
    {
        if (sender is not Button buton || buton.BindingContext is not Attraction atractie)
            return;

        var excursii = await AppDatabase.IaToateExcursiile();
        if (excursii.Count == 0)
        {
            await DisplayAlert("Itinerariu", "Creează mai întâi o călătorie, apoi adaugă obiectivul.", "OK");
            return;
        }

        string eticheta(Trip trip) => $"{trip.NumeDestinatie} ({trip.DataPlecare:dd.MM.yyyy})";
        var optiuni = excursii.Select(eticheta).ToArray();
        var aleasa = await DisplayActionSheet("Adaugă la itinerariul:", "Anulează", null, optiuni);

        if (string.IsNullOrEmpty(aleasa) || aleasa == "Anulează")
            return;

        var calatorie = excursii.FirstOrDefault(t => eticheta(t) == aleasa);
        if (calatorie == null)
            return;

        var zile = Enumerable.Range(1, calatorie.NumarZile)
            .Select(numar => (Numar: numar, Data: calatorie.PrimaZi.AddDays(numar - 1)))
            .ToList();
        var optiuniZile = zile.Select(z => $"Ziua {z.Numar} ({z.Data:dd.MM.yyyy})").ToArray();
        var ziAleasa = await DisplayActionSheet("În ce zi o adaugi?", "Anulează", null, optiuniZile);

        if (string.IsNullOrEmpty(ziAleasa) || ziAleasa == "Anulează")
            return;

        int indexZi = Array.IndexOf(optiuniZile, ziAleasa);
        if (indexZi < 0)
            return;

        var zi = zile[indexZi];
        if (await AppDatabase.ExistaInItinerariu(calatorie.Id, atractie.Nume, zi.Data, calatorie.PrimaZi))
        {
            await DisplayAlert("Itinerariu", $"{atractie.Nume} este deja în ziua {zi.Numar} pentru {calatorie.NumeDestinatie}.", "OK");
            return;
        }

        var locatie = string.IsNullOrWhiteSpace(atractie.Tara)
            ? atractie.Oras
            : $"{atractie.Oras}, {atractie.Tara}";

        await AppDatabase.AdaugaInItinerariu(new ItineraryItem
        {
            TripId = calatorie.Id,
            Nume = atractie.Nume,
            Tip = atractie.Tara,
            Oras = atractie.Oras,
            Data = zi.Data,
            Observatii = locatie
        });

        await DisplayAlert("Itinerariu", $"{atractie.Nume} a fost adăugat în ziua {zi.Numar} pentru {calatorie.NumeDestinatie}.", "OK");
    }
}
