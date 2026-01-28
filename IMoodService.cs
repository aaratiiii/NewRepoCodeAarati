using Aarati_s_Journal.Data;

namespace Aarati_s_Journal.Interfaces;

public interface IMoodService
{
    Task<List<Mood>> GetAllAsync();
}
