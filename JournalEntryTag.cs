namespace Aarati_s_Journal.Data;

public class JournalEntryTag
{
    public int JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = default!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
