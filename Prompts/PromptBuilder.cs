using WorshipSearch.Models;

namespace WorshipSearch.Prompts;

public interface IPromptBuilder
{
    string BuildEnrichmentPrompt(MusicDocument doc);
}

public class PromptBuilder : IPromptBuilder
{
    public string BuildEnrichmentPrompt(MusicDocument doc)
    {
        return PromptTemplates.EnrichmentTemplate
            .Replace("{title}", doc.Title)
            .Replace("{artist}", doc.Artist)
            .Replace("{lyrics}", doc.Lyrics);
    }
}
