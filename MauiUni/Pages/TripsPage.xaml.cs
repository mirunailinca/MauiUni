using System.Collections.ObjectModel;
using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class TripsPage : ContentPage
{
    public static ObservableCollection<Trip> ListaCalatorii { get; set; } = new ObservableCollection<Trip>();

    public TripsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var excursiiDinDb = await AppDatabase.IaToateExcursiile();
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

    private void OnCautareModificata(object sender, TextChangedEventArgs e)
    {
        string textCautat = e.NewTextValue?.ToLower() ?? "";
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

        var parametri = new Dictionary<string, object>
        {
            { "TripIdSelectat", excursieSelectata.Id }
        };

        await Shell.Current.GoToAsync("TripNotesPage", parametri);

        ((CollectionView)sender).SelectedItem = null;
    }
}