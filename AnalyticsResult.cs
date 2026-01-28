namespace Aarati_s_Journal.Models;

public class AnalyticsResult
{
    public int TotalEntries { get; set; }

    public Dictionary<string, int> MoodCategoryCounts { get; set; } = new(); // Positive/Neutral/Negative
    public string MostFrequentMood { get; set; } = "";

    public Dictionary<string, int> TagCounts { get; set; } = new();
    public Dictionary<string, int> EntryCategoryCounts { get; set; } = new();

    public List<(DateTime Date, int WordCount)> WordTrend { get; set; } = new();

    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<DateTime> MissedDays { get; set; } = new();
}
