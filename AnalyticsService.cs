using Aarati_s_Journal.Data;
using Aarati_s_Journal.Interfaces;
using Aarati_s_Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Aarati_s_Journal.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _db;
    private readonly IStreakService _streaks;

    public AnalyticsService(AppDbContext db, IStreakService streaks)
    {
        _db = db;
        _streaks = streaks;
    }

    public async Task<AnalyticsResult> GetAnalyticsAsync(DateTime? from = null, DateTime? to = null)
    {
        var q = _db.JournalEntries
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .AsQueryable();

        if (from.HasValue) q = q.Where(e => e.EntryDate >= from.Value.Date);
        if (to.HasValue) q = q.Where(e => e.EntryDate <= to.Value.Date);

        var entries = await q.OrderBy(e => e.EntryDate).ToListAsync();

        var result = new AnalyticsResult();
        result.TotalEntries = entries.Count;

        // Mood distribution by category (use PRIMARY mood only for analytics)
        var moodCategoryCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var moodNameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var e in entries)
        {
            var primary = e.EntryMoods.FirstOrDefault(m => m.IsPrimary)?.Mood;
            if (primary != null)
            {
                if (!moodCategoryCounts.ContainsKey(primary.Category)) moodCategoryCounts[primary.Category] = 0;
                moodCategoryCounts[primary.Category]++;

                if (!moodNameCounts.ContainsKey(primary.Name)) moodNameCounts[primary.Name] = 0;
                moodNameCounts[primary.Name]++;
            }
        }

        result.MoodCategoryCounts = moodCategoryCounts;
        result.MostFrequentMood = moodNameCounts.Count == 0
            ? ""
            : moodNameCounts.OrderByDescending(x => x.Value).First().Key;

        // Most used tags
        var tagCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            foreach (var t in e.EntryTags.Select(x => x.Tag.Name))
            {
                if (!tagCounts.ContainsKey(t)) tagCounts[t] = 0;
                tagCounts[t]++;
            }
        }
        result.TagCounts = tagCounts;

        // Entry category breakdown
        var catCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            var c = string.IsNullOrWhiteSpace(e.Category) ? "General" : e.Category.Trim();
            if (!catCounts.ContainsKey(c)) catCounts[c] = 0;
            catCounts[c]++;
        }
        result.EntryCategoryCounts = catCounts;

        // Word count trend
        var trend = new List<(DateTime Date, int WordCount)>();
        foreach (var e in entries)
        {
            var wc = CountWords(e.ContentMarkdown);
            trend.Add((e.EntryDate, wc));
        }
        result.WordTrend = trend;

        // streaks
        var streak = await _streaks.ComputeAsync(from, to);
        result.CurrentStreak = streak.CurrentStreak;
        result.LongestStreak = streak.LongestStreak;
        result.MissedDays = streak.MissedDays;

        return result;
    }

    private static int CountWords(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        var parts = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length;
    }
}
