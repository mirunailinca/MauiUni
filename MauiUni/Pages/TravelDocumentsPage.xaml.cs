using System;
using System.Collections.Generic;
using System.Text;
using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

[QueryProperty(nameof(TripId), "TripIdSelectat")]
public partial class TravelDocumentsPage : ContentPage
{
    public int TripId { get; set; }

    private string caleFisierSelectat = string.Empty;
    private string numeFisierSelectat = string.Empty;

    public TravelDocumentsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var excursie = await AppDatabase.IaExcursie(TripId);

        DestinatieLabel.Text = excursie == null
            ? "Documentele călătoriei"
            : $"Călătorie: {excursie.NumeDestinatie}";

        await IncarcaDocumente();
    }

    private async Task IncarcaDocumente()
    {
        DocumenteView.ItemsSource =
            await AppDatabase.IaDocumentePentruExcursie(TripId);
    }

    private async void OnAtaseazaClicked(object sender, EventArgs e)
    {
        try
        {
            var rezultat = await FilePicker.Default.PickAsync();

            if (rezultat == null)
                return;

            string folder = Path.Combine(
                FileSystem.AppDataDirectory,
                "DocumenteCalatorii");

            Directory.CreateDirectory(folder);

            string numeUnic = $"{Guid.NewGuid()}_{Path.GetFileName(rezultat.FileName)}";

            caleFisierSelectat = Path.Combine(folder, numeUnic);

            using var stream = await rezultat.OpenReadAsync();
            using var destinatie = File.Create(caleFisierSelectat);

            await stream.CopyToAsync(destinatie);

            numeFisierSelectat = rezultat.FileName;

            FisierLabel.Text = $"Selectat: {numeFisierSelectat}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare",
                $"Nu s-a putut atașa fișierul: {ex.Message}", "OK");
        }
    }

    private async void OnSalveazaClicked(object sender, EventArgs e)
    {
        if (TransportPicker.SelectedItem == null)
        {
            await DisplayAlert("Atenție",
                "Selectează tipul transportului.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(CodEntry.Text))
        {
            await DisplayAlert("Atenție",
                "Introdu codul rezervării.", "OK");
            return;
        }

        if(string.IsNullOrWhiteSpace(numeFisierSelectat))
        {
            await DisplayAlert("Atenție",
                "Atașează biletul.", "OK");
            return;
        }

        var document = new TravelDocument
        {
            TripId = TripId,
            TipTransport = TransportPicker.SelectedItem.ToString()!,
            CodRezervare = CodEntry.Text.Trim(),
            Locatie = LocatieEntry.Text?.Trim() ?? "",
            NumeFisier = numeFisierSelectat,
            CaleFisier = caleFisierSelectat
        };

        await AppDatabase.AdaugaDocument(document);

        CodEntry.Text = "";
        LocatieEntry.Text = "";
        TransportPicker.SelectedItem = null;
        FisierLabel.Text = "Nu ai selectat niciun fișier.";

        caleFisierSelectat = "";
        numeFisierSelectat = "";

        await IncarcaDocumente();

        await DisplayAlert("Succes",
            "Documentul a fost salvat.", "OK");
    }
    private async void OnDeschideDocumentClicked(
    object sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not TravelDocument document)
            return;

        if (string.IsNullOrWhiteSpace(document.CaleFisier) ||
            !File.Exists(document.CaleFisier))
        {
            await DisplayAlert(
                "Document indisponibil",
                "Fișierul nu a fost găsit.",
                "OK");

            return;
        }

        try
        {
            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(document.CaleFisier)
                });
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Eroare",
                $"Documentul nu a putut fi deschis: {ex.Message}",
                "OK");
        }
    }
}
