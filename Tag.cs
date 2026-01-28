namespace Aarati_s_Journal.Data;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsPrebuilt { get; set; }

    public List<JournalEntryTag> EntryTags { get; set; } = new();
}
