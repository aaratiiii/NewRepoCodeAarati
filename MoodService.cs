using Aarati_s_Journal.Data;
using Aarati_s_Journal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aarati_s_Journal.Services;

public class MoodService : IMoodService
{
    private readonly AppDbContext _db;
    public MoodService(AppDbContext db) => _db = db;

    public Task<List<Mood>> GetAllAsync()
        => _db.Moods.OrderBy(m => m.Category).ThenBy(m => m.Name).ToListAsync();
}
