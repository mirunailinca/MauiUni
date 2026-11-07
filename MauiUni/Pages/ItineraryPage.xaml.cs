using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

[QueryProperty(nameof(TripId), "TripIdSelectat")]
public partial class ItineraryPage : ContentPage
{
    public int TripId { get; set; }

    Trip? _trip;
    bool _pickerPregatit;

    public ItineraryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaZile();
    }

    private async Task IncarcaZile()
    {
        _trip = await AppDatabase.IaExcursie(TripId);
        if (_trip == null)
        {
            await DisplayAlert("Itinerariu", "Vacanța nu a fost găsită.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        TitluLabel.Text = $"Zilele vacanței · {_trip.NumeDestinatie}";

        if (!_pickerPregatit)
        {
            ZiPicker.MinimumDate = new DateTime(2000, 1, 1);
            ZiPicker.MaximumDate = new DateTime(2100, 1, 1);
            ZiPicker.Date = _trip.PrimaZi;
            ZiPicker.MinimumDate = _trip.PrimaZi;
            ZiPicker.MaximumDate = _trip.UltimaZi;
            _pickerPregatit = true;
        }

        var intrari = await AppDatabase.IaItinerariuPentruExcursie(TripId);
        var zile = new List<ZiItinerariu>();

        for (int i = 0; i < _trip.NumarZile; i++)
        {
            var data = _trip.PrimaZi.AddDays(i);
            var aleZilei = intrari
                .Where(item => item.EsteInZiua(data, _trip.PrimaZi))
                .OrderBy(item => string.IsNullOrWhiteSpace(item.Ora) ? "99:99" : item.Ora)
                .Select(item => item.Afisare)
                .ToList();

            zile.Add(new ZiItinerariu
            {
                Numar = i + 1,
                Data = data,
                Continut = aleZilei.Count == 0 ? "-" : string.Join("\n", aleZilei)
            });
        }

        ListaZileView.ItemsSource = zile;
    }

    private async void OnAdaugaInPlanClicked(object sender, EventArgs e)
    {
        if (_trip == null)
            return;

        if (string.IsNullOrWhiteSpace(TitluEntry.Text))
        {
            await DisplayAlert("Itinerariu", "Te rog introdu titlul activității.", "OK");
            return;
        }

        var data = ZiPicker.Date.GetValueOrDefault(_trip.PrimaZi).Date;
        if (data < _trip.PrimaZi || data > _trip.UltimaZi)
            data = _trip.PrimaZi;

        var ora = OraPicker.Time.GetValueOrDefault();
        await AppDatabase.AdaugaInItinerariu(new ItineraryItem
        {
            TripId = TripId,
            Nume = TitluEntry.Text.Trim(),
            Tip = "Activitate",
            Oras = _trip.NumeDestinatie ?? string.Empty,
            Data = data,
            Ora = $"{ora.Hours:00}:{ora.Minutes:00}",
            Observatii = ObservatiiEditor.Text?.Trim() ?? string.Empty
        });

        TitluEntry.Text = string.Empty;
        ObservatiiEditor.Text = string.Empty;
        await IncarcaZile();
    }
}
