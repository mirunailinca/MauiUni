using SQLite;

namespace MauiUni.Models;

public class Trip
{
    // Spunem bazei de date că acesta este ID-ul principal și se generează automat
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NumeDestinatie { get; set; }
    public DateTime DataPlecare { get; set; }
    public DateTime DataIntoarcere { get; set; }
    public string Moneda { get; set; }
    public decimal Buget { get; set; }

    public string DetaliiAfisare => $"{NumeDestinatie} - {DataPlecare:dd/MM/yyyy} ({Buget} {Moneda})";
}