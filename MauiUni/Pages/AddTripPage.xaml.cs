using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class AddTripPage : ContentPage
{
    public AddTripPage()
    {
        InitializeComponent();
    }

    private async void OnSalveazaClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DestinatieEntry.Text))
        {
            await DisplayAlert("Eroare", "Te rog introdu numele destinației!", "OK");
            return;
        }

        var excursieNoua = new Trip
        {
            NumeDestinatie = DestinatieEntry.Text,
            DataPlecare = DataPlecarePicker.Date.GetValueOrDefault(DateTime.Now),
            DataIntoarcere = DataIntoarcerePicker.Date.GetValueOrDefault(DateTime.Now),
            Moneda = MonedaPicker.SelectedItem?.ToString() ?? "RON",
            Buget = decimal.TryParse(BugetEntry.Text, out decimal buget) ? buget : 0
        };

        // ȘTERGE linia veche cu ListaCalatorii.Add() și folosește baza de date:
        await AppDatabase.AdaugaExcursie(excursieNoua);

        await Shell.Current.GoToAsync("..");
    }
}