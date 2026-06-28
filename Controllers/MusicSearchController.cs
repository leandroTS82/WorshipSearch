using Microsoft.AspNetCore.Mvc;
using WorshipSearch.Services;

namespace WorshipSearch.Controllers;

public class MusicSearchController : Controller
{
    private readonly IMusicSearchService _musicSearchService;

    public MusicSearchController(IMusicSearchService musicSearchService)
    {
        _musicSearchService = musicSearchService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Results(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return RedirectToAction(nameof(Index));

        var results = await _musicSearchService.SearchAsync(q);
        ViewBag.Query = q;
        return View(results);
    }

    [HttpGet]
    public async Task<IActionResult> SearchApi(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Json(new List<object>());

        var results = await _musicSearchService.SearchAsync(q);
        return Json(results.Select(r => new
        {
            r.Title,
            r.Artist,
            r.Genre,
            r.ArtistSlug,
            r.SongSlug,
            r.Image,
            r.Popularity,
            r.Url
        }));
    }
}
