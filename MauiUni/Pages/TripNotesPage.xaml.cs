using MauiUni.Models;
using MauiUni.Data;

namespace MauiUni.Pages;

[QueryProperty(nameof(TripId), "TripIdSelectat")]
public partial class TripNotesPage : ContentPage
{
    public int TripId { get; set; }

    public TripNotesPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaNotite();
    }

    private async void OnItinerariuClicked(object sender, EventArgs e)
    {
        var parametri = new Dictionary<string, object>
        {
            { "TripIdSelectat", TripId }
        };

        await Shell.Current.GoToAsync("ItineraryPage", parametri);
    }

    private async void OnBagajClicked(object sender, EventArgs e)
    {
        var parametri = new Dictionary<string, object>
        {
            { "TripIdSelectat", TripId }
        };

        await Shell.Current.GoToAsync("PackingChecklistPage", parametri);
    }

    private async Task IncarcaNotite()
    {
        var notite = await AppDatabase.IaNotitePentruExcursie(TripId);
        ListaNotiteView.ItemsSource = notite;
    }

    private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        int numarCaractere = e.NewTextValue?.Length ?? 0;
        ContorLabel.Text = $"{numarCaractere}/500 caractere";
        ContorLabel.TextColor = numarCaractere >= 490 ? Colors.Red : Colors.Gray;
    }

    private async void OnSalveazaNotitaClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SubiectEntry.Text))
        {
            await DisplayAlert("Eroare", "Te rog introdu un subiect pentru notiță!", "OK");
            return;
        }

        var notitaNoua = new TripNote
        {
            TripId = this.TripId,
            Subiect = SubiectEntry.Text,
            Text = NotitaEditor.Text,
            EsteFavorita = FavoritSwitch.IsToggled
        };

        await AppDatabase.AdaugaNotita(notitaNoua);

        SubiectEntry.Text = string.Empty;
        NotitaEditor.Text = string.Empty;
        FavoritSwitch.IsToggled = false;

        await IncarcaNotite();
    }

    private async void OnDocumenteClicked(object sender, EventArgs e)
    {
        var parametri = new Dictionary<string, object>
    {
        { "TripIdSelectat", TripId }
    };

        await Shell.Current.GoToAsync("TravelDocumentsPage", parametri);
    }

    private async void OnCheltuieliClicked(object sender, EventArgs e)
    {
        var parametri = new Dictionary<string, object>
    {
        { "TripIdSelectat", TripId }
    };

        await Shell.Current.GoToAsync("ExpensesPage", parametri);
    }

    
}