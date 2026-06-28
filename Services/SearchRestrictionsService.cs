using System.Text.Json;
using WorshipSearch.Models;

namespace WorshipSearch.Services;

public interface ISearchRestrictionsService
{
    Task<SearchRestrictions> LoadAsync();
    Task SaveAsync(SearchRestrictions restrictions);
}

public class SearchRestrictionsService : ISearchRestrictionsService
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public SearchRestrictionsService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "restrictions.json");
    }

    public async Task<SearchRestrictions> LoadAsync()
    {
        if (!File.Exists(_filePath)) return new SearchRestrictions();
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<SearchRestrictions>(json) ?? new SearchRestrictions();
    }

    public async Task SaveAsync(SearchRestrictions restrictions)
    {
        var json = JsonSerializer.Serialize(restrictions, JsonOpts);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
