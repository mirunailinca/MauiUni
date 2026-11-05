using SQLite;

namespace MauiUni.Models;

public class TripNote
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Acesta este câmpul care leagă notița de excursia corectă
    public int TripId { get; set; }

    public string Subiect { get; set; }
    public string Text { get; set; }
    public bool EsteFavorita { get; set; }
}