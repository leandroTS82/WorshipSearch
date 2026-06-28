using Microsoft.AspNetCore.Mvc;
using WorshipSearch.Models;
using WorshipSearch.Services;

namespace WorshipSearch.Controllers;

public class HomeController : Controller
{
    private readonly IJsonSearchEngine _searchEngine;

    public HomeController(IJsonSearchEngine searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public async Task<IActionResult> Index(string? q, bool approvedOnly = false)
    {
        var query = new SearchQuery { Query = q ?? string.Empty, ApprovedOnly = approvedOnly };
        var results = await _searchEngine.SearchAsync(query);
        ViewBag.Query = q;
        ViewBag.ApprovedOnly = approvedOnly;
        return View(results);
    }
}
