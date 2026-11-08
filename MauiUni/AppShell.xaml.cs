using MauiUni.Pages;

namespace MauiUni;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("TripNotesPage", typeof(Pages.TripNotesPage));
        Routing.RegisterRoute("AddTripPage", typeof(Pages.AddTripPage));
        Routing.RegisterRoute("WeatherForecastPage", typeof(Pages.WeatherForecastPage));
        Routing.RegisterRoute("ExploreAttractionsPage", typeof(Pages.ExploreAttractionsPage));
        Routing.RegisterRoute("ItineraryPage", typeof(Pages.ItineraryPage));
        Routing.RegisterRoute("PackingChecklistPage", typeof(Pages.PackingChecklistPage));
        Routing.RegisterRoute("CurrencyConverterPage", typeof(Pages.CurrencyConverterPage));
        Routing.RegisterRoute("TravelDocumentsPage", typeof(Pages.TravelDocumentsPage));
        Routing.RegisterRoute("ExpensesPage", typeof(Pages.ExpensesPage));
        Routing.RegisterRoute("BudgetAnalyticsPage", typeof(Pages.BudgetAnalyticsPage));
        Routing.RegisterRoute("TravelPhrasesPage", typeof(Pages.BudgetAnalyticsPage));
    }
}