namespace Aarati_s_Journal.Interfaces;

public interface IPdfExportService
{
    Task<string> ExportAsync(DateTime from, DateTime to);
}
