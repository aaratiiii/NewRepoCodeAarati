using Aarati_s_Journal.Interfaces;

namespace Aarati_s_Journal.Helpers;

public class LoggerService : ILoggerService
{
    public void Info(string message) => System.Diagnostics.Debug.WriteLine("[INFO] " + message);
    public void Error(string message, Exception? ex = null)
        => System.Diagnostics.Debug.WriteLine("[ERROR] " + message + " " + ex);
}
