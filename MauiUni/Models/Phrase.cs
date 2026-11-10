namespace MauiUni.Models;

public class Phrase
{
    public string Categorie { get; set; } = string.Empty;
    public string FrazaOriginala { get; set; } = string.Empty;
    public Dictionary<string, string> Traduceri { get; set; } = new();
    
    public string TraducereCurenta { get; set; } = string.Empty;
}
