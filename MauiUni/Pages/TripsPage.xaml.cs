using System.Collections.ObjectModel;
using System.Linq; // Necesar pentru filtrarea căutării
using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class TripsPage : ContentPage
{
    // Lista noastră principală rămâne statică
    public static ObservableCollection<Trip> ListaCalatorii { get; set; } = new ObservableCollection<Trip>();

    public TripsPage()
    {
        InitializeComponent();
        // Am șters BindingContext = this; deoarece legăm lista manual mai jos
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Citim excursiile reale din baza de date
        var excursiiDinDb = await AppDatabase.IaToateExcursiile();

        // Actualizăm interfața vizuală
        ListaExcursiiView.ItemsSource = excursiiDinDb;

        int numarExcursii = excursiiDinDb.Count;
        if (numarExcursii == 1)
        {
            TitluStatistici.Text = "Salut! Ai o excursie salvată.";
        }
        else
        {
            TitluStatistici.Text = $"Salut! Ai {numarExcursii} excursii salvate.";
        }
    }

    // Se rulează de fiecare dată când tastezi o literă nouă în SearchBar
    private void OnCautareModificata(object sender, TextChangedEventArgs e)
    {
        // Luăm textul introdus și îl facem cu litere mici
        string textCautat = e.NewTextValue?.ToLower() ?? "";

        // Dacă textul e gol, arătăm toate călătoriile, altfel le filtrăm
        if (string.IsNullOrWhiteSpace(textCautat))
        {
            ListaExcursiiView.ItemsSource = ListaCalatorii;
        }
        else
        {
            var excursiiFiltrate = ListaCalatorii
                .Where(trip => trip.NumeDestinatie.ToLower().Contains(textCautat))
                .ToList();

            ListaExcursiiView.ItemsSource = excursiiFiltrate;
        }
    }

    private async void OnAdaugaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AddTripPage");
    }

    private async void OnVeziVremeaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("WeatherForecastPage");
    }

    private async void OnDescoperaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ExploreAttractionsPage");
    }

    private async void OnExcursieSelectata(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Trip excursieSelectata)
            return;

        // Creăm un dicționar cu datele pe care vrem să le trimitem
        var parametri = new Dictionary<string, object>
        {
            { "TripIdSelectat", excursieSelectata.Id }
        };

        // Trimitem parametrii mai departe către pagina de notițe
        await Shell.Current.GoToAsync("TripNotesPage", parametri);

        ((CollectionView)sender).SelectedItem = null;
    }
}