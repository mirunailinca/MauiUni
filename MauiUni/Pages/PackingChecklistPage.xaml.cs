using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

[QueryProperty(nameof(TripId), "TripIdSelectat")]
public partial class PackingChecklistPage : ContentPage
{
    public int TripId { get; set; }

    List<PackingItem> _obiecte = new();
    bool _seIncarca;

    public PackingChecklistPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaBagaj();
    }

    private async Task IncarcaBagaj()
    {
        _obiecte = await AppDatabase.IaBagajPentruExcursie(TripId);

        _seIncarca = true;
        ListaBagajView.ItemsSource = null;
        ListaBagajView.ItemsSource = _obiecte;
        _seIncarca = false;
    }

    private async void OnAdaugaClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DenumireEntry.Text))
        {
            await DisplayAlert("Bagaj", "Te rog introdu denumirea obiectului.", "OK");
            return;
        }

        var obiect = new PackingItem
        {
            TripId = TripId,
            Denumire = DenumireEntry.Text.Trim(),
            Categorie = CategoriePicker.SelectedItem?.ToString() ?? "Haine",
            Impachetat = false
        };

        await AppDatabase.AdaugaObiectBagaj(obiect);
        DenumireEntry.Text = string.Empty;
        await IncarcaBagaj();
    }

    private async void OnBifat(object sender, CheckedChangedEventArgs e)
    {
        if (_seIncarca)
            return;

        if (sender is not CheckBox box || box.BindingContext is not PackingItem obiect)
            return;

        obiect.Impachetat = e.Value;
        await AppDatabase.ActualizeazaObiectBagaj(obiect);
    }
}
