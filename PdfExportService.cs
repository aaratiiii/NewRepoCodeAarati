using Aarati_s_Journal.Data;
using Aarati_s_Journal.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Aarati_s_Journal.Services;

// No external packages: export HTML file.
// User opens it and uses "Print -> Save as PDF".
public class PdfExportService : IPdfExportService
{
    private readonly AppDbContext _db;
    public PdfExportService(AppDbContext db) => _db = db;

    public async Task<string> ExportAsync(DateTime from, DateTime to)
    {
        var f = from.Date;
        var t = to.Date;

        var entries = await _db.JournalEntries
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .Where(e => e.EntryDate >= f && e.EntryDate <= t)
            .OrderBy(e => e.EntryDate)
            .ToListAsync();

        var outPath = Path.Combine(FileSystem.AppDataDirectory, $"Journal_{f:yyyyMMdd}_{t:yyyyMMdd}.html");

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html><html><head><meta charset='utf-8'>");
        sb.AppendLine("<title>Journal Export</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("body{font-family:Arial,Helvetica,sans-serif;margin:24px;}");
        sb.AppendLine(".entry{border:1px solid #ccc;border-radius:12px;padding:12px;margin:12px 0;}");
        sb.AppendLine(".meta{color:#555;font-size:14px;margin-top:6px;}");
        sb.AppendLine("pre{white-space:pre-wrap;font-family:Consolas,monospace;background:#f6f6f6;padding:10px;border-radius:10px;}");
        sb.AppendLine("</style></head><body>");
        sb.AppendLine($"<h2>Aarati's Journal Export</h2>");
        sb.AppendLine($"<p><b>Range:</b> {f:dd MMM yyyy} - {t:dd MMM yyyy}</p>");

        if (entries.Count == 0)
        {
            sb.AppendLine("<p>No entries in this date range.</p>");
        }
        else
        {
            foreach (var e in entries)
            {
                var primary = e.EntryMoods.FirstOrDefault(x => x.IsPrimary)?.Mood?.Name ?? "";
                var secondary = e.EntryMoods.Where(x => !x.IsPrimary).Select(x => x.Mood.Name).ToList();
                var tags = e.EntryTags.Select(x => x.Tag.Name).ToList();

                sb.AppendLine("<div class='entry'>");
                sb.AppendLine($"<h3>{Escape(e.EntryDate.ToString("dddd, dd MMM yyyy"))}</h3>");
                sb.AppendLine($"<h4>{Escape(string.IsNullOrWhiteSpace(e.Title) ? "(No title)" : e.Title)}</h4>");
                sb.AppendLine("<div class='meta'>");
                sb.AppendLine($"<div><b>Category:</b> {Escape(e.Category)}</div>");
                sb.AppendLine($"<div><b>Primary Mood:</b> {Escape(primary)}</div>");
                if (secondary.Count > 0) sb.AppendLine($"<div><b>Secondary:</b> {Escape(string.Join(", ", secondary))}</div>");
                if (tags.Count > 0) sb.AppendLine($"<div><b>Tags:</b> {Escape(string.Join(", ", tags))}</div>");
                sb.AppendLine($"<div><b>Created:</b> {e.CreatedAt:dd MMM yyyy HH:mm} | <b>Updated:</b> {e.UpdatedAt:dd MMM yyyy HH:mm}</div>");
                sb.AppendLine("</div>");

                sb.AppendLine("<h5>Content (Markdown text)</h5>");
                sb.AppendLine($"<pre>{Escape(e.ContentMarkdown ?? "")}</pre>");
                sb.AppendLine("</div>");
            }
        }

        sb.AppendLine("<hr/>");
        sb.AppendLine("<p><small>To generate PDF without extra installs: open this HTML file and use Print → Save as PDF.</small></p>");
        sb.AppendLine("</body></html>");

        await File.WriteAllTextAsync(outPath, sb.ToString());
        return outPath;
    }

    private static string Escape(string s)
        => System.Net.WebUtility.HtmlEncode(s ?? "");
}
