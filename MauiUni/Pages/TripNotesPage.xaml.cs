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

    // Se apelează automat când se deschide ecranul
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IncarcaNotite(); // Aici accesează amintirile deja adaugate
    }

    private async Task IncarcaNotite()
    {
        // Caută în baza de date SQLite doar amintirile pentru excursia pe care ai dat click
        var notite = await AppDatabase.IaNotitePentruExcursie(TripId);

        // Le afișează în lista de sus
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

        // Curățăm formularul după salvare
        SubiectEntry.Text = string.Empty;
        NotitaEditor.Text = string.Empty;
        FavoritSwitch.IsToggled = false;

        // Reîncărcăm lista pentru a vizualiza instantaneu noua amintire
        await IncarcaNotite();
    }
}