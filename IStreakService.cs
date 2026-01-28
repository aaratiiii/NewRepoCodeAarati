using Aarati_s_Journal.Models;

namespace Aarati_s_Journal.Interfaces;

public interface IStreakService
{
    Task<UserStreak> ComputeAsync(DateTime? from = null, DateTime? to = null);
}
