using Aarati_s_Journal.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Aarati_s_Journal.Services;

public class AuthService : IAuthService
{
    private const string PinHashKey = "PIN_SHA256";

    public Task<bool> IsPinSetAsync()
    {
        var hash = Preferences.Get(PinHashKey, "");
        return Task.FromResult(!string.IsNullOrWhiteSpace(hash));
    }

    public Task SetPinAsync(string pin)
    {
        var hash = Sha256Hex(pin);
        Preferences.Set(PinHashKey, hash);
        return Task.CompletedTask;
    }

    public Task<bool> VerifyPinAsync(string pin)
    {
        var stored = Preferences.Get(PinHashKey, "");
        if (string.IsNullOrWhiteSpace(stored)) return Task.FromResult(false);

        var incoming = Sha256Hex(pin);
        return Task.FromResult(string.Equals(stored, incoming, StringComparison.OrdinalIgnoreCase));
    }

    public Task ClearPinAsync()
    {
        Preferences.Remove(PinHashKey);
        return Task.CompletedTask;
    }

    private static string Sha256Hex(string input)
    {
        input ??= "";
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
