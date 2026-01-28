namespace Aarati_s_Journal.Data;

public class JournalEntry
{
    public int Id { get; set; }

    // Date-only meaning, store as DateTime but use Date part.
    public DateTime EntryDate { get; set; }

    public string Title { get; set; } = "";
    public string ContentMarkdown { get; set; } = "";

    // Category for the entry (e.g., Work, Health, etc. – separate from MoodCategory)
    public string Category { get; set; } = "General";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<JournalEntryMood> EntryMoods { get; set; } = new();
    public List<JournalEntryTag> EntryTags { get; set; } = new();
}
