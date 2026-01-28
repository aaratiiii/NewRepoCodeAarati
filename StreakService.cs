using Aarati_s_Journal.Data;
using Aarati_s_Journal.Interfaces;
using Aarati_s_Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Aarati_s_Journal.Services;

public class StreakService : IStreakService
{
    private readonly AppDbContext _db;
    public StreakService(AppDbContext db) => _db = db;

    public async Task<UserStreak> ComputeAsync(DateTime? from = null, DateTime? to = null)
    {
        // streaks should consider from earliest entry to today if not specified
        var entries = await _db.JournalEntries
            .Select(e => e.EntryDate)
            .OrderBy(d => d)
            .ToListAsync();

        if (entries.Count == 0) return new UserStreak();

        var start = (from?.Date) ?? entries.First();
        var end = (to?.Date) ?? DateTime.Today;

        start = start.Date;
        end = end.Date;

        var entrySet = entries.Select(d => d.Date).ToHashSet();

        // missed days list
        var missed = new List<DateTime>();
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (!entrySet.Contains(d)) missed.Add(d);
        }

        // current streak ending today
        int current = 0;
        for (var d = DateTime.Today; d >= start; d = d.AddDays(-1))
        {
            if (entrySet.Contains(d)) current++;
            else break;
        }

        // longest streak
        int longest = 0, run = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (entrySet.Contains(d))
            {
                run++;
                if (run > longest) longest = run;
            }
            else run = 0;
        }

        return new UserStreak
        {
            CurrentStreak = current,
            LongestStreak = longest,
            MissedDays = missed
        };
    }
}
