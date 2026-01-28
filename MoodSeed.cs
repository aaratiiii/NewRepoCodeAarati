namespace Aarati_s_Journal.Data;

public static class MoodSeed
{
    public static readonly (string Category, string[] Moods)[] All =
    {
        ("Positive", new[] {"Happy","Excited","Relaxed","Grateful","Confident"}),
        ("Neutral",  new[] {"Calm","Thoughtful","Curious","Nostalgic","Bored"}),
        ("Negative", new[] {"Sad","Angry","Stressed","Lonely","Anxious"})
    };
}
