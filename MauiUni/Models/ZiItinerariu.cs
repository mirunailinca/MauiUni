namespace MauiUni.Models;

public class ZiItinerariu
{
    public int Numar { get; set; }
    public DateTime Data { get; set; }
    public string Continut { get; set; } = "-";

    public string Titlu => $"Ziua {Numar} · {Data:dd MMM yyyy}";
}
