namespace Aarati_s_Journal.Interfaces;

public interface IThemeService
{
    bool IsDark { get; }
    void Toggle();
}
