using SQLite;

namespace MauiUni.Models;

public class ItineraryItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int TripId { get; set; }
    public string Nume { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
    public string Oras { get; set; } = string.Empty;
    public decimal PretBilet { get; set; }

    public DateTime Data { get; set; }
    public string Ora { get; set; } = string.Empty;
    public string Observatii { get; set; } = string.Empty;

    public string Afisare
    {
        get
        {
            var linie = string.IsNullOrWhiteSpace(Ora) ? Nume : $"{Ora}  {Nume}";
            if (!string.IsNullOrWhiteSpace(Observatii))
                linie += $" — {Observatii}";
            return linie;
        }
    }

    public bool EsteInZiua(DateTime zi, DateTime primaZi)
    {
        if (Data.Year < 2000)
            return zi.Date == primaZi.Date;

        return Data.Date == zi.Date;
    }
}
