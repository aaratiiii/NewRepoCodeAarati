using System.ComponentModel.DataAnnotations;

namespace Aarati_s_Journal.Models
{
    // Optional: keep this if your DB still references User table.
    // Single-user app: this is just profile info (no auth).
    public class User
    {
        public int Id { get; set; } = 1;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = "Aarati";

        // Optional fields (no longer required)
        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Streak tracking properties (if you still use them)
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public DateOnly? LastEntryDate { get; set; }
    }
}
