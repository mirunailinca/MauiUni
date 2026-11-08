using SQLite;

namespace MauiUni.Models;

public class Trip
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string NumeDestinatie { get; set; }
    public DateTime DataPlecare { get; set; }
    public DateTime DataIntoarcere { get; set; }
    public string Moneda { get; set; }
    public decimal Buget { get; set; }

    public string DetaliiAfisare => $"{NumeDestinatie} - {DataPlecare:dd/MM/yyyy} ({Buget} {Moneda})";

    public DateTime PrimaZi => DataPlecare.Date;

    public DateTime UltimaZi => DataIntoarcere.Date < DataPlecare.Date
        ? DataPlecare.Date
        : DataIntoarcere.Date;

    public int NumarZile => (UltimaZi - PrimaZi).Days + 1;
}