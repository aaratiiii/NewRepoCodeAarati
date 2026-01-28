using Aarati_s_Journal.Data;
using Aarati_s_Journal.Interfaces;

namespace Aarati_s_Journal.Services;

public class DatabaseService : IDatabaseService
{
    private readonly AppDbContext _db;

    public DatabaseService(AppDbContext db) => _db = db;

    public Task EnsureCreatedAsync()
    {
        _db.Database.EnsureCreated();
        SeedData.EnsureSeeded(_db);
        return Task.CompletedTask;
    }
}
