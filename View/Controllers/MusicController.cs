using System.IO.Compression;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WertyMusic.Requests;
using System.Text.Json;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace View.Controllers;

public class MusicController : Controller
{
    private readonly IHubContext<MusicHub> _hubContext;
    private readonly IMemoryCache _cache;
    
    private readonly IMusicService _musicService;

    public MusicController(IMusicService musicService,IHubContext<MusicHub> hubContext, IMemoryCache cache)
    {
        _musicService = musicService;
        _hubContext = hubContext;
        _cache = cache;
    }
    
    [HttpPost]
    public async Task<IActionResult> FindMusic([FromBody] FindRequest request)
    {
        _ = Task.Run(async () =>
        {
            var foundIds = new List<Guid>();

            await foreach (var music in _musicService.FindMusicsAsync(request.MusicName))
            {
                foundIds.Add(music.Id);
                
                await _hubContext.Clients.Client(request.ConnectionId)
                    .SendAsync("ReceiveMusic", music);
            }

            _cache.Set($"results_{request.ConnectionId}", foundIds, TimeSpan.FromMinutes(30));
        
            await _hubContext.Clients.Client(request.ConnectionId)
                .SendAsync("SearchFinished");
        });
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> FindMusic()
    {
        return View("BeautyFindMusic");
    }
    
    [HttpGet]
    public async Task<IActionResult> AllMusics()
    {
        var results = await _musicService.GetMusicsAsync();
            
        return View("BeautyAllMusics",results);
    }

    [HttpPost]
    public async Task<IActionResult> DownloadZip([FromBody] DownloadRequest request)
    {
        var ids = _cache.Get($"results_{request.ConnectionId}") as List<Guid>;
        
        var zipFile = await _musicService.DownloadMusicsAsync(ids);
        
        return File(zipFile, "application/zip", "archive.zip");
    }
}