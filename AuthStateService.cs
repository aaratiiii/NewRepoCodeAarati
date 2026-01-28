using Aarati_s_Journal.Interfaces;

namespace Aarati_s_Journal.Services;

public class AuthStateService : IAuthStateService
{
    private const string Key = "AUTH_UNLOCKED";

    public bool IsAuthenticated { get; private set; } = Preferences.Get(Key, false);

    public void SetAuthenticated(bool value)
    {
        IsAuthenticated = value;
        Preferences.Set(Key, value);
    }
}
