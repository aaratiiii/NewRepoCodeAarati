namespace Aarati_s_Journal.Data;

public class Mood
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = ""; // Positive / Neutral / Negative

    public List<JournalEntryMood> EntryMoods { get; set; } = new();
}
