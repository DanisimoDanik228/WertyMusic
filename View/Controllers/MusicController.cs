using System.IO.Compression;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WertyMusic.Requests;

namespace View.Controllers;

public class MusicController : Controller
{
    private const string FoundMusicIdsKey = "FoundMusicIds";
    private readonly IMusicService _musicService;

    public MusicController(IMusicService musicService)
    {
        _musicService = musicService;
    }

    
    [HttpPost]
    public async Task<IActionResult> FindMusic(string query)
    {
        var results = await _musicService.FindMusicsAsync(query);

        TempData[FoundMusicIdsKey] = System.Text.Json.JsonSerializer.Serialize(results.Select(x => x.Id));
        
        return View(results);
    }
    
    [HttpGet]
    public async Task<IActionResult> FindMusic()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> AllMusics()
    {
        var results = await _musicService.GetMusicsAsync();
            
        return View(results);
    }

    [HttpPost]
    public async Task<IActionResult> DownloadZip()
    {
        var idsJson = TempData[FoundMusicIdsKey] as string;
        var ids = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(idsJson);
        var zipFile = await _musicService.DownloadMusicsAsync(ids);
        
        return File(zipFile, "application/zip", "archive.zip");
    }
}