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
    
    [HttpPost("musics")]
    public async Task<IActionResult> FindMusics([FromBody] FindRequest request)
    {
        IEnumerable<Music> listMusic = await _musicService.FindMusicsAsync(request.musicName);
        
        return Ok(listMusic);
    }
    
    [HttpGet("allmusics")]
    public async Task<IActionResult> GetAllMusics()
    {
        IEnumerable<Music> listMusic = await _musicService.GetMusicsAsync();
        
        return Ok(listMusic);
    }
    
    //Download, add to repository and send .mp3 file
    [HttpPost("download")]
    public async Task<IActionResult> Download([FromBody] DonwloadRequest request)
    {
        var res = await _musicService.DownloadMusicsAsync(request.musicsId);

        var zipPath =  @"C:\Users\Werty\source\repos\Code\C#\WertyMusic\Presentation\bin\Debug\net9.0\Storage\Source_A\1.zip";
        CreateZipFromFileList(res,zipPath);
        
        var fileBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
        
        System.IO.File.Delete(zipPath);
        
        return File(fileBytes, "application/zip", "archive.zip");
    }
    
    [HttpGet("music/{id}")]
    public async Task<IActionResult> GetMusicFile(Guid id)
    {
        var stream = await _musicService.GetFileMusicAsync(id);
        
        return File(stream, "audio/mpeg","temp_name" + ".mp3");
    }
    
    static void CreateZipFromFileList(IEnumerable<string> filePaths, string zipPath)
    {
        using (FileStream zipStream = new FileStream(zipPath, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (string filePath in filePaths)
            {
                if (System.IO.File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    archive.CreateEntryFromFile(filePath, fileName);
                    Console.WriteLine($"Добавлен файл: {fileName}");
                }
                else
                {
                    Console.WriteLine($"Файл не найден: {filePath}");
                }
            }
        }
    }
}