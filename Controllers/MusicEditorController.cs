using Microsoft.AspNetCore.Mvc;
using WorshipSearch.Models;
using WorshipSearch.Services;

namespace WorshipSearch.Controllers;

public class MusicEditorController : Controller
{
    private readonly IMusicRepository _repository;
    private readonly IMusicEnrichmentService _enrichment;

    public MusicEditorController(IMusicRepository repository, IMusicEnrichmentService enrichment)
    {
        _repository = repository;
        _enrichment = enrichment;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string fileName)
    {
        var doc = await _repository.LoadAsync(fileName);
        if (doc == null) return NotFound();

        ViewBag.FileName = fileName;
        return View(doc);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] MusicDocument doc, [FromQuery] string fileName)
    {
        try
        {
            await _repository.SaveAsync(doc);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Enrich([FromBody] MusicDocument doc)
    {
        try
        {
            var enriched = await _enrichment.EnrichAsync(doc);
            return Json(enriched);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
