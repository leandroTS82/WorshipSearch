namespace WorshipSearch.Prompts;

public static class PromptTemplates
{
    public const string EnrichmentSystemInstruction = @"You are a worship music analyst. Analyze the song and return ONLY valid JSON with no extra text, no markdown code blocks, no explanation.";

    public const string EnrichmentTemplate = @"Analyze this worship song and return a JSON object with these fields:
- themes: array of main theological/worship themes (strings)
- contexts: array of worship contexts where this song fits (e.g., ""abertura"", ""comunhão"", ""intercessão"", ""ceia"", ""encerramento"")
- keywords: array of keywords for search
- biblical_topics: array of biblical topics covered
- biblical_references: array of specific Bible verse references (e.g., ""João 3:16"")
- biblical_books: array of Bible books referenced or alluded to
- biblical_characters: array of biblical figures mentioned or alluded to
- moods: array of emotional moods (e.g., ""adoração"", ""gratidão"", ""clamor"", ""celebração"")
- synonyms: array of synonym search terms
- worship_style: string describing worship style (e.g., ""contemporâneo"", ""tradicional"", ""pentecostal"")
- energy_level: string - one of: ""baixa"", ""média"", ""alta""
- occasion: string describing main occasion (e.g., ""culto dominical"", ""conferência de jovens"", ""oração"")
- summary: 2-3 sentence summary of the song's message
- explanation: deeper theological explanation (3-5 sentences)
- practical_application: how to use this song in worship (2-3 sentences)

Song title: {title}
Artist: {artist}
Lyrics:
{lyrics}

Return ONLY the JSON object, no other text.";
}
