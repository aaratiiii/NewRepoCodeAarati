namespace Aarati_s_Journal.Interfaces;

public interface IAuthStateService
{
    bool IsAuthenticated { get; }
    void SetAuthenticated(bool value);
}
