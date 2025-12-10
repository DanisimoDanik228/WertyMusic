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
    private readonly IMusicService _musicService;

    public MusicController(IMusicService musicService)
    {
        _musicService = musicService;
    }
    
    // [HttpPost("musics")]
    // public async Task<IActionResult> FindMusics([FromBody] FindRequest request)
    // {
    //     IEnumerable<Music> listMusic = await _musicService.FindMusicsAsync(request.musicName);
    //     
    //     return Ok(listMusic);
    // }
    //
    // [HttpGet("musics")]
    // public async Task<IActionResult> GetAllMusics()
    // {
    //     var listMusic = await _musicService.GetMusicsAsync();
    //     
    //     return Ok(listMusic);
    // }
    //
    // [HttpPost("download")]
    // public async Task<IActionResult> Download([FromBody] DonwloadRequest request)
    // {
    //     var zipFile = await _musicService.DownloadMusicsAsync(request.musicsId);
    //     
    //     return File(zipFile, "application/zip", "archive.zip");
    // }
    //
    // [HttpGet("music/{id}")]
    // public async Task<IActionResult> GetMusicFile(Guid id)
    // {
    //     var stream = await _musicService.GetFileMusicAsync(id);
    //     
    //     return File(stream, "audio/mpeg","temp_name" + ".mp3");
    // }




    [HttpPost]
    public async Task<IActionResult> FindMusic(string query)
    {
        var results = await _musicService.FindMusicsAsync(query);
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
}