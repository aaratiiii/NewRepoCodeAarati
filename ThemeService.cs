using Aarati_s_Journal.Interfaces;

namespace Aarati_s_Journal.Services;

public class ThemeService : IThemeService
{
    private const string Key = "THEME_DARK";

    public bool IsDark => Preferences.Get(Key, false);

    public void Toggle()
    {
        Preferences.Set(Key, !IsDark);
    }
}
