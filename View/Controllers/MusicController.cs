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

    private const string _findMusicView = "BeautifulMusic/BeautyFindMusic"; 
    //private const string _findMusicView = "FindMusic";
    private const string _allMusicView = "BeautifulMusic/BeautyAllMusics";
    //private const string _allMusicView = "AllMusics";
    
    private const string FoundMusicIdsKey = "FoundMusicIds";
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
                Console.WriteLine("FindMusic: " + music.MusicName);
                foundIds.Add(music.Id);

                // Отправляем песню клиенту через SignalR
                await _hubContext.Clients.Client(request.ConnectionId)
                    .SendAsync("ReceiveMusic", music);
            }

            _cache.Set($"results_{request.ConnectionId}", foundIds, TimeSpan.FromHours(1));
        
            await _hubContext.Clients.Client(request.ConnectionId)
                .SendAsync("SearchFinished");
        });
        return Ok();
        //return View(_findMusicView,results);
    }
    
    [HttpGet]
    public async Task<IActionResult> FindMusic()
    {
        return View(_findMusicView);
    }
    
    [HttpGet]
    public async Task<IActionResult> AllMusics()
    {
        var results = await _musicService.GetMusicsAsync();
            
        return View(_allMusicView,results);
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