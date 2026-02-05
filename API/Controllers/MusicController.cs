using System.IO.Compression;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WertyMusic.Requests;

namespace WertyMusic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase 
{
    private readonly IMusicService _musicService;

    public MusicController(IMusicService musicService)
    {
        _musicService = musicService;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("musics")]
    public async Task<IActionResult> FindMusics([FromBody] FindRequest request)
    {
        var listMusic = _musicService.FindMusicsAsync(request.MusicName);
        var res = new List<Music>();

        await foreach (var music in listMusic)
        {
            res.Add(music);
        }
        
        return Ok(res);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("musics")]
    public async Task<IActionResult> GetAllMusics()
    {
        var listMusic = await _musicService.GetMusicsAsync();
        
        return Ok(listMusic);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("download")]
    public async Task<IActionResult> Download([FromBody] DownloadRequest request)
    {
        var zipFile = await _musicService.DownloadMusicsAsync(request.musicsId);

        if (zipFile == null)
        {
            return NotFound();
        }

        return File(zipFile, "application/zip", "archive.zip");
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("music/{id}")]
    public async Task<IActionResult> GetMusicFile(Guid id)
    {
        var stream = await _musicService.GetFileMusicAsync(id);
        
        if (stream == null || stream.Length == 0)
        {
            return NotFound();
        }
        
        return File(stream, "audio/mpeg","temp_name" + ".mp3");
    }
}