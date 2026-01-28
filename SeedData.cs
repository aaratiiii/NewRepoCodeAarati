using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Aarati_s_Journal.Data
{
    public static class SeedData
    {
        public static void EnsureSeeded(AppDbContext db)
        {
            db.Database.EnsureCreated();

            SeedMoods(db);

            // Tags are optional. Only seed them if Tag doesn't have required shadow columns.
            TrySeedTags(db);
        }

        private static void SeedMoods(AppDbContext db)
        {
            if (db.Moods.Any())
                return;

            foreach (var group in MoodSeed.All)
            {
                var category = (group.Category ?? "").Trim();
                if (string.IsNullOrWhiteSpace(category))
                    category = "General";

                foreach (var moodNameRaw in group.Moods ?? Array.Empty<string>())
                {
                    var moodName = (moodNameRaw ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(moodName))
                        continue;

                    db.Moods.Add(new Mood
                    {
                        Name = moodName,
                        Category = category
                    });
                }
            }

            db.SaveChanges();
        }

        private static void TrySeedTags(AppDbContext db)
        {
            if (db.Tags.Any())
                return;

            // Detect required shadow properties on Tag (NOT NULL columns not represented as C# properties)
            var tagEntityType = db.Model.FindEntityType(typeof(Tag));
            if (tagEntityType == null)
                return;

            var hasRequiredShadowNotNull = tagEntityType
                .GetProperties()
                .Any(p => !p.IsNullable && p.PropertyInfo == null && !p.IsPrimaryKey());

            if (hasRequiredShadowNotNull)
            {
                // Your Tag table requires extra NOT NULL fields that are not in Tag.cs,
                // so seeding tags will always crash. Skip it so app can start.
                return;
            }

            // If no required shadow NOT NULL columns, safe to seed basic tags
            var defaults = new[] { "Work", "Health", "Family", "Study", "Gratitude" };

            foreach (var t in defaults)
            {
                var name = (t ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                db.Tags.Add(new Tag
                {
                    Name = name,
                    IsPrebuilt = true
                });
            }

            db.SaveChanges();
        }
    }
}
