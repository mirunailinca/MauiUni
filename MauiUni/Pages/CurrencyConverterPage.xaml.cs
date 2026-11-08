using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

public partial class CurrencyConverterPage : ContentPage
{
    private static readonly HttpClient Client = new HttpClient();

    private const string UrlCursCurent =
        "https://curs.bnr.ro/nbrfxrates.xml";

    private const string UrlUltimele10Zile =
        "https://curs.bnr.ro/nbrfxrates10days.xml";

    private Dictionary<string, decimal> _cursuri = new();

    public CurrencyConverterPage()
    {
        InitializeComponent();

        MonedaSursaPicker.SelectedItem = "EUR";
        MonedaDestinatiePicker.SelectedItem = "RON";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await IncarcaCursuri();
    }

    private async Task IncarcaCursuri()
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            string xml = await Client.GetStringAsync(UrlCursCurent);

            using var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);

            _cursuri.Clear();
            _cursuri["RON"] = 1m;

            string dataCurs = "";

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.LocalName == "Cube" &&
                    reader.GetAttribute("date") != null)
                {
                    dataCurs = reader.GetAttribute("date") ?? "";
                }

                else if (reader.NodeType == XmlNodeType.Element &&
                         reader.LocalName == "Rate")
                {
                    string moneda =
                        reader.GetAttribute("currency") ?? "";

                    string multiplierText =
                        reader.GetAttribute("multiplier") ?? "1";

                    string valoareText =
                        reader.ReadElementContentAsString();

                    if (decimal.TryParse(
                        valoareText,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out decimal curs))
                    {
                        decimal multiplier = 1m;

                        decimal.TryParse(
                            multiplierText,
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out multiplier);

                        if (multiplier <= 0)
                            multiplier = 1m;

                        if (!string.IsNullOrWhiteSpace(moneda))
                        {
                            _cursuri[moneda] = curs / multiplier;
                        }
                    }

                    continue;
                }

                reader.Read();
            }

            if (_cursuri.Count <= 1)
                throw new Exception("Nu au fost găsite cursuri valutare.");

            var monede = _cursuri.Keys
                .OrderBy(x => x)
                .ToList();

            MonedaSursaPicker.ItemsSource = monede;
            MonedaDestinatiePicker.ItemsSource = monede;

            MonedaSursaPicker.SelectedItem = "EUR";
            MonedaDestinatiePicker.SelectedItem = "RON";

            DataCursLabel.Text =
                $"Ultima actualizare BNR: {dataCurs}";

            StatusLabel.Text =
                $"Au fost încărcate {_cursuri.Count - 1} monede.";

            await IncarcaIstoric();
        }
        catch (Exception ex)
        {
            StatusLabel.Text =
                "Nu s-au putut prelua cursurile BNR.";

            await DisplayAlert(
                "Eroare",
                $"Nu am putut accesa serviciul BNR.\n{ex.Message}",
                "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void OnConvertesteClicked(object sender, EventArgs e)
    {
        if (_cursuri.Count == 0)
        {
            DisplayAlert(
                "Atenție",
                "Cursurile BNR nu sunt încă disponibile.",
                "OK");

            return;
        }

        if (!decimal.TryParse(
            SumaEntry.Text,
            NumberStyles.Number,
            CultureInfo.CurrentCulture,
            out decimal suma))
        {
            DisplayAlert(
                "Eroare",
                "Introdu o sumă validă.",
                "OK");

            return;
        }

        if (suma < 0)
        {
            DisplayAlert(
                "Eroare",
                "Suma nu poate fi negativă.",
                "OK");

            return;
        }

        string sursa =
            MonedaSursaPicker.SelectedItem?.ToString() ?? "EUR";

        string destinatie =
            MonedaDestinatiePicker.SelectedItem?.ToString() ?? "RON";

        if (!_cursuri.ContainsKey(sursa) ||
            !_cursuri.ContainsKey(destinatie))
        {
            DisplayAlert(
                "Eroare",
                "Moneda selectată nu este disponibilă.",
                "OK");

            return;
        }

        // Conversie prin RON.
        decimal valoareRon = suma * _cursuri[sursa];

        decimal rezultat =
            valoareRon / _cursuri[destinatie];

        RezultatLabel.Text =
            $"{rezultat:N2} {destinatie}";

        DetaliiLabel.Text =
            $"{suma:N2} {sursa} = {rezultat:N2} {destinatie}";

        decimal cursConversie =
            _cursuri[sursa] / _cursuri[destinatie];

        CursConversieLabel.Text =
            $"1 {sursa} = {cursConversie:N4} {destinatie}";
    }

    private void OnInverseazaClicked(object sender, EventArgs e)
    {
        var sursa = MonedaSursaPicker.SelectedItem;
        var destinatie = MonedaDestinatiePicker.SelectedItem;

        MonedaSursaPicker.SelectedItem = destinatie;
        MonedaDestinatiePicker.SelectedItem = sursa;

        OnConvertesteClicked(sender, e);
    }

    private async Task IncarcaIstoric()
    {
        try
        {
            string xml =
                await Client.GetStringAsync(UrlUltimele10Zile);

            using var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);

            var istoric = new List<string>();

            string dataCurs = "";

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.LocalName == "Cube" &&
                    reader.GetAttribute("date") != null)
                {
                    dataCurs = reader.GetAttribute("date") ?? "";
                }

                else if (reader.NodeType == XmlNodeType.Element &&
                         reader.LocalName == "Rate" &&
                         reader.GetAttribute("currency") == "EUR")
                {
                    string multiplierText =
                        reader.GetAttribute("multiplier") ?? "1";

                    string valoareText =
                        reader.ReadElementContentAsString();

                    if (decimal.TryParse(
                        valoareText,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out decimal cursEuro))
                    {
                        decimal multiplier = 1m;

                        decimal.TryParse(
                            multiplierText,
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out multiplier);

                        if (multiplier <= 0)
                            multiplier = 1m;

                        cursEuro /= multiplier;

                        istoric.Add(
                            $"{dataCurs}     1 EUR = {cursEuro:N4} RON");
                    }

                    continue;
                }

                reader.Read();
            }

            IstoricView.ItemsSource = istoric;
        }
        catch
        {
            IstoricView.ItemsSource = new List<string>
            {
                "Istoricul cursurilor nu este disponibil momentan."
            };
        }
    }
}