namespace Aarati_s_Journal.Data;

public static class DbPatches
{
    public static void Apply(AppDbContext db)
    {
        SeedData.EnsureSeeded(db);
    }
}
