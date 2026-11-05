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
}