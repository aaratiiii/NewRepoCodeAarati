namespace Aarati_s_Journal.Models;

public class SearchParameters
{
    public string? Query { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public List<int> MoodIds { get; set; } = new(); // filter moods
    public List<int> TagIds { get; set; } = new();  // filter tags
}
