using Microsoft.EntityFrameworkCore;

namespace Aarati_s_Journal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<JournalEntryTag> JournalEntryTags => Set<JournalEntryTag>();
    public DbSet<Mood> Moods => Set<Mood>();
    public DbSet<JournalEntryMood> JournalEntryMoods => Set<JournalEntryMood>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.EntryDate)
            .IsUnique(); // one journal entry per day

        modelBuilder.Entity<JournalEntryTag>()
            .HasKey(x => new { x.JournalEntryId, x.TagId });

        modelBuilder.Entity<JournalEntryMood>()
            .HasKey(x => new { x.JournalEntryId, x.MoodId });

        modelBuilder.Entity<JournalEntryTag>()
            .HasOne(x => x.JournalEntry)
            .WithMany(e => e.EntryTags)
            .HasForeignKey(x => x.JournalEntryId);

        modelBuilder.Entity<JournalEntryTag>()
            .HasOne(x => x.Tag)
            .WithMany(t => t.EntryTags)
            .HasForeignKey(x => x.TagId);

        modelBuilder.Entity<JournalEntryMood>()
            .HasOne(x => x.JournalEntry)
            .WithMany(e => e.EntryMoods)
            .HasForeignKey(x => x.JournalEntryId);

        modelBuilder.Entity<JournalEntryMood>()
            .HasOne(x => x.Mood)
            .WithMany(m => m.EntryMoods)
            .HasForeignKey(x => x.MoodId);
    }
}
