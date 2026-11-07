using SQLite;

namespace MauiUni.Models;

public class PackingItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int TripId { get; set; }
    public string Denumire { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public bool Impachetat { get; set; }
}
