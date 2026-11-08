using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace MauiUni.Models;

public class TravelDocument
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int TripId { get; set; }

    public string TipTransport { get; set; } = string.Empty;

    public string CodRezervare { get; set; } = string.Empty;

    public string Locatie { get; set; } = string.Empty;

    public string NumeFisier { get; set; } = string.Empty;

    public string CaleFisier { get; set; } = string.Empty;
}

