namespace Aarati_s_Journal.Interfaces;

public interface IAuthService
{
    Task<bool> IsPinSetAsync();
    Task SetPinAsync(string pin);
    Task<bool> VerifyPinAsync(string pin);
    Task ClearPinAsync();
}
