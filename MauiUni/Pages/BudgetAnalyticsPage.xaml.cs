using Microcharts;
using SkiaSharp;
using MauiUni.Data;
using MauiUni.Models;

namespace MauiUni.Pages;

[QueryProperty(nameof(TripId), "TripIdSelectat")]
public partial class BudgetAnalyticsPage : ContentPage
{
    public int TripId { get; set; }
    private Trip? _trip;
    public BudgetAnalyticsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _trip = await AppDatabase.IaExcursie(TripId);
        var cheltuieli = await AppDatabase.IaCheltuieli(TripId);

        if (_trip != null)
        {
            DestinatieAnalizaLabel.Text = $"Călătorie: {_trip.NumeDestinatie}";
        }

        CalculeazaSiAfiseazaDatele(cheltuieli);
    }

    private void CalculeazaSiAfiseazaDatele(List<Expense> cheltuieli)
    {
        decimal totalCheltuit = cheltuieli.Sum(x => x.Suma);
        decimal bugetTotal = _trip?.Buget ?? 0;
        string moneda = _trip?.Moneda ?? "RON";

        TotalCheltuitLabel.Text = $"{totalCheltuit:N2} {moneda}";

        if (bugetTotal > 0)
        {
            double progres = (double)(totalCheltuit / bugetTotal);
            BugetProgressBar.Progress = Math.Min(progres, 1.0);
            ProcentBugetLabel.Text = $"{progres * 100:F1}% din bugetul de {bugetTotal:N2} {moneda} consumat";
        }
        else
        {
            BugetProgressBar.Progress = 0;
            ProcentBugetLabel.Text = "Nu a fost setat un buget estimat pentru această călătorie.";
        }

        // light/dark mode pentru legenda grafice
        bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        SKColor textColor = isDarkMode ? SKColor.Parse("#E5E7EB") : SKColor.Parse("#374151");

        var groupedData = cheltuieli
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Categorie) ? "Altele" : x.Categorie)
            .Select(g => new ChartEntry((float)g.Sum(x => x.Suma))
            {
                Label = g.Key,
                ValueLabel = $"{g.Sum(x => x.Suma):N2}",
                Color = SKColor.Parse(GetColorForCategory(g.Key)),
                ValueLabelColor = textColor
            })
            .ToList();

        if (groupedData.Count == 0)
        {
            groupedData.Add(new ChartEntry(0) { Label = "Fără cheltuieli", ValueLabel = "0", Color = SKColor.Parse("#9CA3AF") });
        }



        CategoryChart.Chart = new DonutChart
        {
            Entries = groupedData,
            AnimationDuration = TimeSpan.FromSeconds(1),
            LabelTextSize = 32,
            Margin = 18,
            HoleRadius = 0.5f,
            BackgroundColor = SKColor.Empty,
            LabelColor = textColor
        };

        var payerData = cheltuieli
            .GroupBy(x => string.IsNullOrWhiteSpace(x.PlatitDe) ? "Necunoscut" : x.PlatitDe)
            .Select((g, index) => new ChartEntry((float)g.Sum(x => x.Suma))
            {
                Label = g.Key,
                ValueLabel = $"{g.Sum(x => x.Suma):N2}",
                Color = SKColor.Parse(GetDistinctColor(index)),
                ValueLabelColor = textColor
            })
            .ToList();

        if (payerData.Count == 0)
        {
            payerData.Add(new ChartEntry(0) { Label = "Niciun plătitor", ValueLabel = "0", Color = SKColor.Parse("#9CA3AF") });
        }

        PayerChart.Chart = new DonutChart
        {
            Entries = payerData,
            AnimationDuration = TimeSpan.FromSeconds(1),
            LabelTextSize = 32,
            Margin = 15,
            HoleRadius = 0.5f,
            BackgroundColor = SKColor.Empty,
            LabelColor = textColor
        };
    }

    private string GetColorForCategory(string category)
    {
        return category.ToLower() switch
        {
            "cazare" => "#2563EB",
            "mâncare" or "mancare" => "#DC2626",
            "transport" => "#D97706",
            "activități" or "activitati" => "#059669", 
            _ => "#7C3AED"
            
        };
    }

    private string GetDistinctColor(int index)
    {
        string[] palette = { "#2563EB", "#059669", "#D97706", "#DC2626", "#7C3AED", "#DB2777", "#0284C7" };
        return palette[index % palette.Length];
    }
}