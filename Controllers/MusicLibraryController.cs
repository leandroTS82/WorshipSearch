using Microsoft.AspNetCore.Mvc;
using WorshipSearch.Models;
using WorshipSearch.Services;

namespace WorshipSearch.Controllers;

public class MusicLibraryController : Controller
{
    private readonly IMusicRepository _repository;
    private readonly IJsonSearchEngine _searchEngine;

    public MusicLibraryController(IMusicRepository repository, IJsonSearchEngine searchEngine)
    {
        _repository = repository;
        _searchEngine = searchEngine;
    }

    public async Task<IActionResult> Index(string? q, bool approvedOnly = false)
    {
        var query = new SearchQuery { Query = q ?? string.Empty, ApprovedOnly = approvedOnly, PageSize = 50 };
        var results = await _searchEngine.SearchAsync(query);
        ViewBag.Query = q;
        ViewBag.ApprovedOnly = approvedOnly;
        return View(results);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string fileName)
    {
        await _repository.DeleteAsync(fileName);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleApproved(string fileName)
    {
        var doc = await _repository.LoadAsync(fileName);
        if (doc == null) return NotFound();

        doc.Approved = !doc.Approved;
        await _repository.SaveAsync(doc);
        return Json(new { success = true, approved = doc.Approved });
    }
}
