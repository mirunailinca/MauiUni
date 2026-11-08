using SQLite;

namespace MauiUni.Models;

public class Expense
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int TripId { get; set; }
    public decimal Suma { get; set; }
    public string Descriere { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string PlatitDe { get; set; } = string.Empty;
    public string Participanti { get; set; } = string.Empty;
    public DateTime Data { get; set; } = DateTime.Today;
}
