namespace Aarati_s_Journal.Models;

public class UserStreak
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<DateTime> MissedDays { get; set; } = new();
}
