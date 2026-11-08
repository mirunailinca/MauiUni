using System;
using System.Collections.Generic;
using System.Text;
using MauiUni.Data;
using MauiUni.Models;
using System.Globalization;

namespace MauiUni.Pages;

[QueryProperty(nameof(TripId), "TripIdSelectat")]
public partial class ExpensesPage : ContentPage
{
    public int TripId { get; set; }

    private Trip? _trip;

    public ExpensesPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _trip = await AppDatabase.IaExcursie(TripId);

        if (_trip == null)
            return;

        DestinatieLabel.Text = $"Călătorie: {_trip.NumeDestinatie}";

        await IncarcaCheltuieli();
    }

    private async Task IncarcaCheltuieli()
    {
        var lista = await AppDatabase.IaCheltuieli(TripId);

        CheltuieliView.ItemsSource = lista;

        decimal total = lista.Sum(x => x.Suma);

        string moneda = _trip?.Moneda ?? "RON";

        TotalLabel.Text = $"{total:N2} {moneda}";

        decimal buget = _trip?.Buget ?? 0;

        BugetLabel.Text =
            $"Buget disponibil: {buget - total:N2} {moneda}";
    }

    private async void OnAdaugaCheltuialaClicked(
        object sender, EventArgs e)
    {
        if (!decimal.TryParse(
            SumaEntry.Text,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal suma))
        {
            await DisplayAlert("Eroare",
                "Introdu o sumă validă.", "OK");
            return;
        }

        if (suma <= 0)
        {
            await DisplayAlert("Eroare",
                "Suma trebuie să fie mai mare decât zero.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(DescriereEntry.Text) ||
            string.IsNullOrWhiteSpace(PlatitDeEntry.Text) ||
            CategoriePicker.SelectedItem == null ||
            string.IsNullOrWhiteSpace(ParticipantiEntry.Text))
        {
            await DisplayAlert("Eroare",
                "Completează toate câmpurile.", "OK");
            return;
        }

        var participanti = ParticipantiEntry.Text!
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        string platitor = PlatitDeEntry.Text.Trim();

        if (!participanti.Any(x =>
            x.Equals(platitor, StringComparison.OrdinalIgnoreCase)))
        {
            await DisplayAlert("Eroare",
                "Persoana care a plătit trebuie să fie inclusă în lista participanților.",
                "OK");
            return;
        }

        var cheltuiala = new Expense
        {
            TripId = TripId,
            Suma = suma,
            Descriere = DescriereEntry.Text.Trim(),
            Categorie = CategoriePicker.SelectedItem.ToString()!,
            PlatitDe = platitor,
            Participanti = string.Join(", ", participanti),
            Data = DateTime.Today
        };

        await AppDatabase.AdaugaCheltuiala(cheltuiala);

        SumaEntry.Text = "";
        DescriereEntry.Text = "";
        PlatitDeEntry.Text = "";
        ParticipantiEntry.Text = "";
        CategoriePicker.SelectedItem = null;

        await IncarcaCheltuieli();

        await DisplayAlert("Succes",
            "Cheltuiala a fost salvată.", "OK");
    }

    private async void OnCalculeazaClicked(object sender, EventArgs e)
    {
        var cheltuieli = await AppDatabase.IaCheltuieli(TripId);

        if (cheltuieli.Count == 0)
        {
            await DisplayAlert("Informație",
                "Nu există cheltuieli înregistrate.", "OK");
            return;
        }

        var solduri = new Dictionary<string, decimal>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var cheltuiala in cheltuieli)
        {
            var participanti = cheltuiala.Participanti
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (participanti.Count == 0)
                continue;

            decimal cota = cheltuiala.Suma / participanti.Count;

            foreach (var persoana in participanti)
            {
                if (!solduri.ContainsKey(persoana))
                    solduri[persoana] = 0;

                solduri[persoana] -= cota;
            }

            if (!solduri.ContainsKey(cheltuiala.PlatitDe))
                solduri[cheltuiala.PlatitDe] = 0;

            solduri[cheltuiala.PlatitDe] += cheltuiala.Suma;
        }

        string moneda = _trip?.Moneda ?? "RON";

        var rezultat = new List<string>();

        foreach (var persoana in solduri)
        {
            string situatie = persoana.Value switch
            {
                > 0 => $"are de primit {persoana.Value:N2} {moneda}",
                < 0 => $"trebuie să plătească {Math.Abs(persoana.Value):N2} {moneda}",
                _ => "nu are diferențe de plată"
            };

            rezultat.Add($"{persoana.Key}: {situatie}");
        }

        ImpartireLabel.Text = string.Join("\n\n", rezultat);
    }
}
