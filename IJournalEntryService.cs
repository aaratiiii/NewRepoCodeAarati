using Aarati_s_Journal.Data;
using Aarati_s_Journal.Models;

namespace Aarati_s_Journal.Interfaces;

public interface IJournalEntryService
{
    Task<JournalEntry?> GetByDateAsync(DateTime date);
    Task<JournalEntry> UpsertForDateAsync(DateTime date, string title, string markdown, string category,
        int primaryMoodId, List<int> secondaryMoodIds, List<int> tagIds);

    Task DeleteByDateAsync(DateTime date);

    Task<PaginatedResult<JournalEntry>> SearchAsync(SearchParameters p, int page, int pageSize);

    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag> CreateCustomTagAsync(string name);

    Task<List<Mood>> GetAllMoodsAsync();
}
