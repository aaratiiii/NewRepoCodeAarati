namespace Aarati_s_Journal.Data;

public class JournalEntryMood
{
    public int JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = default!;

    public int MoodId { get; set; }
    public Mood Mood { get; set; } = default!;

    public bool IsPrimary { get; set; }
}
