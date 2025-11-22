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
    // add method for get all(.zip) or only one music(.mp3)
    private readonly IMusicService _musicService;

    public MusicController(IMusicService musicService)
    {
        _musicService = musicService;
    }
    
    // Download, add to repository and send .mp3 file
    [HttpPost("download")]
    public async Task<IActionResult> Download([FromBody] DownloadRequest request)
    {
        IEnumerable<Music> listMusic = await _musicService.DownloadMusicsAsync(request.musicName);

        var music = await _musicService.AddMusicAsync(listMusic.First());
        
        var file = await _musicService.GetFileMusicAsync(music.Id);
        
        return File(file, "audio/mpeg", music.Name + ".mp3");
    }
    
    [HttpGet("music/{id}")]
    public async Task<IActionResult> GetMusicFile(Guid id)
    {
        var stream = await _musicService.GetFileMusicAsync(id);
        
        return File(stream, "audio/mpeg","temp_name" + ".mp3");
    }
}