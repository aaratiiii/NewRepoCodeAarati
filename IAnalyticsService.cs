using Aarati_s_Journal.Models;

namespace Aarati_s_Journal.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsResult> GetAnalyticsAsync(DateTime? from = null, DateTime? to = null);
}
