namespace Aarati_s_Journal.Interfaces;

public interface ILoggerService
{
    void Info(string message);
    void Error(string message, Exception? ex = null);
}
