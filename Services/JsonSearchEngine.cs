using WorshipSearch.Models;

namespace WorshipSearch.Services;

public interface IJsonSearchEngine
{
    Task<List<LibrarySearchResult>> SearchAsync(SearchQuery query);
}

public class JsonSearchEngine : IJsonSearchEngine
{
    private readonly IMusicRepository _repository;

    public JsonSearchEngine(IMusicRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LibrarySearchResult>> SearchAsync(SearchQuery query)
    {
        var all = await _repository.LoadAllAsync();
        var results = new List<LibrarySearchResult>();

        var terms = string.IsNullOrWhiteSpace(query.Query)
            ? Array.Empty<string>()
            : query.Query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var (fileName, doc) in all)
        {
            if (query.ApprovedOnly && !doc.Approved) continue;

            double score = string.IsNullOrWhiteSpace(query.Query)
                ? 1.0
                : ScoreDocument(doc, terms);

            if (score > 0 || string.IsNullOrWhiteSpace(query.Query))
            {
                results.Add(new LibrarySearchResult
                {
                    Document = doc,
                    Score = score,
                    FileName = fileName
                });
            }
        }

        return results
            .OrderByDescending(r => r.Score)
            .ThenByDescending(r => r.Document.Approved)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();
    }

    private static double ScoreDocument(MusicDocument doc, string[] terms)
    {
        double total = 0;

        foreach (var term in terms)
        {
            total += ScoreField(doc.Title, term, weight: 10);
            total += ScoreField(doc.Artist, term, weight: 8);
            total += ScoreList(doc.Search.Themes, term, weight: 6);
            total += ScoreList(doc.Search.Keywords, term, weight: 5);
            total += ScoreList(doc.Search.BiblicalReferences, term, weight: 5);
            total += ScoreList(doc.Search.BiblicalTopics, term, weight: 4);
            total += ScoreList(doc.Search.BiblicalBooks, term, weight: 4);
            total += ScoreList(doc.Search.Contexts, term, weight: 4);
            total += ScoreList(doc.Search.Moods, term, weight: 3);
            total += ScoreField(doc.Summary, term, weight: 3);
            total += ScoreField(doc.Search.Occasion, term, weight: 3);
            total += ScoreList(doc.Search.Synonyms, term, weight: 3);
            total += ScoreList(doc.Search.BiblicalCharacters, term, weight: 3);
            total += ScoreField(doc.Lyrics, term, weight: 2);
            total += ScoreField(doc.Explanation, term, weight: 2);
            total += ScoreField(doc.PracticalApplication, term, weight: 1);
        }

        return total;
    }

    private static double ScoreField(string field, string term, double weight)
    {
        if (string.IsNullOrWhiteSpace(field)) return 0;
        var lower = field.ToLowerInvariant();
        if (lower == term) return weight * 2;
        if (lower.Contains(term)) return weight;
        return 0;
    }

    private static double ScoreList(List<string> list, string term, double weight)
    {
        double score = 0;
        foreach (var item in list)
            score += ScoreField(item, term, weight);
        return score;
    }
}
