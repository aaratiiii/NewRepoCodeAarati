using Aarati_s_Journal.Models;

namespace Aarati_s_Journal.Models;

public class JournalSearchRequest
{
    public int UserId { get; set; }

    public string? Text { get; set; }              // content/title search
    public DateOnly? From { get; set; }            // date range
    public DateOnly? To { get; set; }

    public int? PrimaryMoodId { get; set; }
    public List<int>? AnyMoodIds { get; set; }     // match primary or secondary
    public List<string>? Tags { get; set; }        // match any tag
    public JournalCategory? Category { get; set; } // optional
}
