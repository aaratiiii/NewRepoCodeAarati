using Aarati_s_Journal.Data;
using Aarati_s_Journal.Exceptions;
using Aarati_s_Journal.Helpers;
using Aarati_s_Journal.Interfaces;
using Aarati_s_Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Aarati_s_Journal.Services;

public class JournalEntryService : IJournalEntryService
{
    private readonly AppDbContext _db;

    public JournalEntryService(AppDbContext db) => _db = db;

    public async Task<JournalEntry?> GetByDateAsync(DateTime date)
    {
        var d = date.Date;
        return await _db.JournalEntries
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.EntryDate == d);
    }

    public async Task<JournalEntry> UpsertForDateAsync(DateTime date, string title, string markdown, string category,
        int primaryMoodId, List<int> secondaryMoodIds, List<int> tagIds)
    {
        var d = date.Date;

        ValidationHelper.Require(primaryMoodId > 0, "Primary mood is required.");
        secondaryMoodIds = secondaryMoodIds?.Distinct().Where(x => x > 0 && x != primaryMoodId).ToList() ?? new();
        ValidationHelper.Require(secondaryMoodIds.Count <= Constants.MaxSecondaryMoods, "You can pick up to 2 secondary moods.");

        var primaryMood = await _db.Moods.FindAsync(primaryMoodId);
        if (primaryMood == null) throw new NotFoundException("Primary mood not found.");

        foreach (var sid in secondaryMoodIds)
        {
            if (await _db.Moods.FindAsync(sid) == null)
                throw new NotFoundException("Secondary mood not found.");
        }

        var entry = await _db.JournalEntries
            .Include(e => e.EntryMoods)
            .Include(e => e.EntryTags)
            .FirstOrDefaultAsync(e => e.EntryDate == d);

        var now = DateTime.Now;

        if (entry == null)
        {
            entry = new JournalEntry
            {
                EntryDate = d,
                Title = title?.Trim() ?? "",
                ContentMarkdown = markdown ?? "",
                Category = string.IsNullOrWhiteSpace(category) ? "General" : category.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.JournalEntries.Add(entry);
            await _db.SaveChangesAsync();
        }
        else
        {
            entry.Title = title?.Trim() ?? "";
            entry.ContentMarkdown = markdown ?? "";
            entry.Category = string.IsNullOrWhiteSpace(category) ? "General" : category.Trim();
            entry.UpdatedAt = now;
        }

        // Replace moods
        entry.EntryMoods.Clear();
        entry.EntryMoods.Add(new JournalEntryMood { JournalEntryId = entry.Id, MoodId = primaryMoodId, IsPrimary = true });
        foreach (var sid in secondaryMoodIds)
            entry.EntryMoods.Add(new JournalEntryMood { JournalEntryId = entry.Id, MoodId = sid, IsPrimary = false });

        // Replace tags
        entry.EntryTags.Clear();
        var validTagIds = (tagIds ?? new()).Distinct().Where(x => x > 0).ToList();
        foreach (var tid in validTagIds)
        {
            var tag = await _db.Tags.FindAsync(tid);
            if (tag != null)
                entry.EntryTags.Add(new JournalEntryTag { JournalEntryId = entry.Id, TagId = tid });
        }

        await _db.SaveChangesAsync();

        // Re-load fully
        return (await GetByDateAsync(d))!;
    }

    public async Task DeleteByDateAsync(DateTime date)
    {
        var d = date.Date;
        var entry = await _db.JournalEntries.FirstOrDefaultAsync(e => e.EntryDate == d);
        if (entry == null) return;

        _db.JournalEntries.Remove(entry);
        await _db.SaveChangesAsync();
    }

    public async Task<PaginatedResult<JournalEntry>> SearchAsync(SearchParameters p, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var q = _db.JournalEntries
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(p.Query))
        {
            var term = p.Query.Trim().ToLowerInvariant();
            q = q.Where(e =>
                e.Title.ToLower().Contains(term) ||
                e.ContentMarkdown.ToLower().Contains(term));
        }

        if (p.From.HasValue)
        {
            var from = p.From.Value.Date;
            q = q.Where(e => e.EntryDate >= from);
        }

        if (p.To.HasValue)
        {
            var to = p.To.Value.Date;
            q = q.Where(e => e.EntryDate <= to);
        }

        if (p.MoodIds.Count > 0)
        {
            q = q.Where(e => e.EntryMoods.Any(m => p.MoodIds.Contains(m.MoodId)));
        }

        if (p.TagIds.Count > 0)
        {
            q = q.Where(e => e.EntryTags.Any(t => p.TagIds.Contains(t.TagId)));
        }

        q = q.OrderByDescending(e => e.EntryDate);

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResult<JournalEntry>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<List<Tag>> GetAllTagsAsync()
        => _db.Tags.OrderByDescending(t => t.IsPrebuilt).ThenBy(t => t.Name).ToListAsync();

    public async Task<Tag> CreateCustomTagAsync(string name)
    {
        name = (name ?? "").Trim();
        ValidationHelper.Require(name.Length >= 2, "Tag name too short.");

        var exists = await _db.Tags.AnyAsync(t => t.Name.ToLower() == name.ToLower());
        if (exists) throw new DuplicateEntryException("Tag already exists.");

        var tag = new Tag { Name = name, IsPrebuilt = false };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();
        return tag;
    }

    public Task<List<Mood>> GetAllMoodsAsync()
        => _db.Moods.OrderBy(m => m.Category).ThenBy(m => m.Name).ToListAsync();
}
