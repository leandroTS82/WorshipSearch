using System.Text.Json;
using WorshipSearch.Models;

namespace WorshipSearch.Services;

public interface IMusicRepository
{
    Task SaveAsync(MusicDocument doc);
    Task<MusicDocument?> LoadAsync(string fileName);
    Task<List<(string FileName, MusicDocument Doc)>> LoadAllAsync();
    Task DeleteAsync(string fileName);
    string GetFileName(MusicDocument doc);
    bool Exists(string fileName);
}

public class MusicRepository : IMusicRepository
{
    private readonly string _jsonFolder;
    private readonly ILogger<MusicRepository> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public MusicRepository(IWebHostEnvironment env, ILogger<MusicRepository> logger)
    {
        _jsonFolder = Path.Combine(env.ContentRootPath, "json");
        _logger = logger;
        Directory.CreateDirectory(_jsonFolder);
    }

    public string GetFileName(MusicDocument doc) => $"louvor-{doc.Id}.json";

    public bool Exists(string fileName) => File.Exists(Path.Combine(_jsonFolder, fileName));

    public async Task SaveAsync(MusicDocument doc)
    {
        var fileName = GetFileName(doc);
        var filePath = Path.Combine(_jsonFolder, fileName);
        var json = JsonSerializer.Serialize(doc, JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
        _logger.LogInformation("Saved {FileName}", fileName);
    }

    public async Task<MusicDocument?> LoadAsync(string fileName)
    {
        var filePath = Path.Combine(_jsonFolder, fileName);
        if (!File.Exists(filePath)) return null;

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<MusicDocument>(json);
    }

    public async Task<List<(string FileName, MusicDocument Doc)>> LoadAllAsync()
    {
        var files = Directory.GetFiles(_jsonFolder, "*.json");
        var results = new List<(string, MusicDocument)>();

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var doc = JsonSerializer.Deserialize<MusicDocument>(json);
                if (doc != null)
                    results.Add((Path.GetFileName(file), doc));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load {File}", file);
            }
        }

        return results;
    }

    public Task DeleteAsync(string fileName)
    {
        var filePath = Path.Combine(_jsonFolder, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }
}
