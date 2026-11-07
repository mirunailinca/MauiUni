namespace MauiUni;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Înregistrăm ruta pentru pagina a doua
        Routing.RegisterRoute("TripNotesPage", typeof(Pages.TripNotesPage));
        Routing.RegisterRoute("AddTripPage", typeof(Pages.AddTripPage));
        Routing.RegisterRoute("WeatherForecastPage", typeof(Pages.WeatherForecastPage));
        Routing.RegisterRoute("ExploreAttractionsPage", typeof(Pages.ExploreAttractionsPage));
        Routing.RegisterRoute("ItineraryPage", typeof(Pages.ItineraryPage));
        Routing.RegisterRoute("PackingChecklistPage", typeof(Pages.PackingChecklistPage));
    }
}