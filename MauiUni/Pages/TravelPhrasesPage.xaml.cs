using System.Text.Json;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class TravelPhrasesPage : ContentPage
{
    private List<Phrase> _toateFrazele = new();

    public TravelPhrasesPage()
    {
        InitializeComponent();
        LimbaPicker.SelectedIndex = 0; // Engleza
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaFrazeDinJson();
        PopuleazaCategoriiPicker();
        ActualizeazaLista();
    }

    private async Task IncarcaFrazeDinJson()
    {
        if (_toateFrazele.Count > 0) return;

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("json_fraze_utile.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            _toateFrazele = JsonSerializer.Deserialize<List<Phrase>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Phrase>();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare", $"Nu s-a putut citi JSON-ul: {ex.Message}", "OK");
        }
    }

    private void PopuleazaCategoriiPicker()
    {
        if (_toateFrazele.Count == 0) return;

        List<string> categorii = _toateFrazele
            .Select(f => f.Categorie)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        categorii.Insert(0, "Toate categoriile");

        CategoriePicker.ItemsSource = categorii;
        CategoriePicker.SelectedIndex = 0;
    }

    private void ActualizeazaLista()
    {
        string limbaSelectata = LimbaPicker.SelectedItem?.ToString() ?? "engleza";
        string categoriaSelectata = CategoriePicker.SelectedItem?.ToString() ?? "Toate categoriile";
        string cautare = BaraCautareFraze.Text?.Trim().ToLowerInvariant() ?? "";

        foreach (var item in _toateFrazele)
        {
            if (item.Traduceri.TryGetValue(limbaSelectata, out string? traducere))
            {
                item.TraducereCurenta = traducere;
            }
            else
            {
                item.TraducereCurenta = "Traducere indisponibilă";
            }
        }

        var listaFiltrata = _toateFrazele.Where(f =>
        {
            bool corespundeCategorie = categoriaSelectata == "Toate categoriile" ||
                                       f.Categorie.Equals(categoriaSelectata, StringComparison.OrdinalIgnoreCase);

            bool corespundeCautare = string.IsNullOrWhiteSpace(cautare) ||
                                     f.FrazaOriginala.ToLowerInvariant().Contains(cautare) ||
                                     f.TraducereCurenta.ToLowerInvariant().Contains(cautare);

            return corespundeCategorie && corespundeCautare;
        }).ToList();

        ListaFrazeView.ItemsSource = null;
        ListaFrazeView.ItemsSource = listaFiltrata;
    }

    private void OnFiltruSchimbat(object sender, EventArgs e)
    {
        ActualizeazaLista();
    }

    private void OnCautareTextChanged(object sender, TextChangedEventArgs e)
    {
        ActualizeazaLista();
    }

    private async void OnCopiazaFrazaClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Phrase item)
        {
            await Clipboard.Default.SetTextAsync(item.TraducereCurenta);
            await DisplayAlert("Copiat", $"Traducerea „{item.TraducereCurenta}” a fost copiată în clipboard!", "OK");
        }
    }
}