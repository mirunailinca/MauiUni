using SQLite;
using MauiUni.Models;

namespace MauiUni.Data;

public static class AppDatabase
{
    static SQLiteAsyncConnection _db;

    static async Task Init()
    {
        if (_db != null)
            return;

        string cale = Path.Combine(FileSystem.AppDataDirectory, "TripMate.db3");
        _db = new SQLiteAsyncConnection(cale);

        await _db.CreateTableAsync<Trip>();
        await _db.CreateTableAsync<TripNote>();
        await _db.CreateTableAsync<ItineraryItem>();
        await _db.CreateTableAsync<PackingItem>();
        await _db.CreateTableAsync<TravelDocument>();
        await _db.CreateTableAsync<Expense>();
        await AsiguraColoana("ItineraryItem", "Data", "INTEGER");
        await AsiguraColoana("ItineraryItem", "Ora", "TEXT");
        await AsiguraColoana("ItineraryItem", "Observatii", "TEXT");
    }

    static async Task AsiguraColoana(string tabel, string coloana, string tipSql)
    {
        var coloane = await _db.GetTableInfoAsync(tabel);
        if (coloane.All(c => c.Name != coloana))
            await _db.ExecuteAsync($"ALTER TABLE {tabel} ADD COLUMN {coloana} {tipSql}");
    }

    public static async Task AdaugaExcursie(Trip excursie)
    {
        await Init();
        await _db.InsertAsync(excursie);
    }

    public static async Task<List<Trip>> IaToateExcursiile()
    {
        await Init();
        return await _db.Table<Trip>().ToListAsync();
    }

    public static async Task AdaugaNotita(TripNote notita)
    {
        await Init();
        await _db.InsertAsync(notita);
    }

    public static async Task<List<TripNote>> IaNotitePentruExcursie(int tripId)
    {
        await Init();
        // Caută în baza de date doar notițele care corespund excursiei pe care ai dat click
        return await _db.Table<TripNote>().Where(n => n.TripId == tripId).ToListAsync();
    }

    public static async Task<Trip?> IaExcursie(int id)
    {
        await Init();
        return await _db.Table<Trip>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public static async Task<List<ItineraryItem>> IaItinerariuPentruExcursie(int tripId)
    {
        await Init();
        return await _db.Table<ItineraryItem>().Where(i => i.TripId == tripId).ToListAsync();
    }

    public static async Task<bool> ExistaInItinerariu(int tripId, string nume, DateTime zi, DateTime primaZi)
    {
        var intrari = await IaItinerariuPentruExcursie(tripId);
        return intrari.Any(i => i.Nume == nume && i.EsteInZiua(zi, primaZi));
    }

    public static async Task AdaugaInItinerariu(ItineraryItem obiectiv)
    {
        await Init();
        await _db.InsertAsync(obiectiv);
    }

    public static async Task<List<PackingItem>> IaBagajPentruExcursie(int tripId)
    {
        await Init();
        return await _db.Table<PackingItem>().Where(p => p.TripId == tripId).ToListAsync();
    }

    public static async Task AdaugaObiectBagaj(PackingItem obiect)
    {
        await Init();
        await _db.InsertAsync(obiect);
    }

    public static async Task ActualizeazaObiectBagaj(PackingItem obiect)
    {
        await Init();
        await _db.UpdateAsync(obiect);
    }
    public static async Task AdaugaDocument(TravelDocument document)
    {
        await Init();
        await _db.InsertAsync(document);
    }

    public static async Task<List<TravelDocument>>
        IaDocumentePentruExcursie(int tripId)
    {
        await Init();

        return await _db.Table<TravelDocument>()
            .Where(d => d.TripId == tripId)
            .ToListAsync();
    }

    public static async Task AdaugaCheltuiala(Expense cheltuiala)
    {
        await Init();
        await _db.InsertAsync(cheltuiala);
    }

    public static async Task<List<Expense>> IaCheltuieli(int tripId)
    {
        await Init();

        return await _db.Table<Expense>()
            .Where(e => e.TripId == tripId)
            .ToListAsync();
    }
}