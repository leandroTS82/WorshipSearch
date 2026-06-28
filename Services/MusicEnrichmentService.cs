using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WorshipSearch.Models;
using WorshipSearch.Prompts;

namespace WorshipSearch.Services;

public interface IMusicEnrichmentService
{
    Task<MusicDocument> EnrichAsync(MusicDocument doc);
}

public class MusicEnrichmentService : IMusicEnrichmentService
{
    private readonly IGroqService _groqService;
    private readonly IPromptBuilder _promptBuilder;
    private readonly ILogger<MusicEnrichmentService> _logger;

    public MusicEnrichmentService(IGroqService groqService, IPromptBuilder promptBuilder, ILogger<MusicEnrichmentService> logger)
    {
        _groqService = groqService;
        _promptBuilder = promptBuilder;
        _logger = logger;
    }

    public async Task<MusicDocument> EnrichAsync(MusicDocument doc)
    {
        try
        {
            var prompt = _promptBuilder.BuildEnrichmentPrompt(doc);
            var response = await _groqService.CompleteAsync(prompt);

            ApplyEnrichment(doc, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LLM enrichment failed for {Title}", doc.Title);
            // Return doc without enrichment — fallback
        }

        return doc;
    }

    private void ApplyEnrichment(MusicDocument doc, string llmResponse)
    {
        // Strip markdown code blocks if present
        var cleaned = Regex.Replace(llmResponse, @"```(?:json)?", string.Empty).Trim();

        var jsonNode = JsonNode.Parse(cleaned);
        if (jsonNode == null) return;

        doc.Search.Themes = ParseStringArray(jsonNode["themes"]);
        doc.Search.Contexts = ParseStringArray(jsonNode["contexts"]);
        doc.Search.Keywords = ParseStringArray(jsonNode["keywords"]);
        doc.Search.BiblicalTopics = ParseStringArray(jsonNode["biblical_topics"]);
        doc.Search.BiblicalReferences = ParseStringArray(jsonNode["biblical_references"]);
        doc.Search.BiblicalBooks = ParseStringArray(jsonNode["biblical_books"]);
        doc.Search.BiblicalCharacters = ParseStringArray(jsonNode["biblical_characters"]);
        doc.Search.Moods = ParseStringArray(jsonNode["moods"]);
        doc.Search.Synonyms = ParseStringArray(jsonNode["synonyms"]);
        doc.Search.WorshipStyle = jsonNode["worship_style"]?.GetValue<string>() ?? string.Empty;
        doc.Search.EnergyLevel = jsonNode["energy_level"]?.GetValue<string>() ?? string.Empty;
        doc.Search.Occasion = jsonNode["occasion"]?.GetValue<string>() ?? string.Empty;
        doc.Summary = jsonNode["summary"]?.GetValue<string>() ?? string.Empty;
        doc.Explanation = jsonNode["explanation"]?.GetValue<string>() ?? string.Empty;
        doc.PracticalApplication = jsonNode["practical_application"]?.GetValue<string>() ?? string.Empty;
    }

    private static List<string> ParseStringArray(JsonNode? node)
    {
        if (node is not JsonArray arr) return new List<string>();
        return arr
            .Select(n => n?.GetValue<string>() ?? string.Empty)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }
}
