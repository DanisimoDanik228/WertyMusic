using System.IO.Compression;
using System.Runtime.ExceptionServices;
using ClassLibrary1.Interfaces;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Services;

public class MusicService : IMusicService
{
    private readonly IEnumerable<IMusicDownloadService> _downloadServices;
    private readonly IFileSender _fileSender;
    private readonly IMusicRepository _musicRepository;
    
    public MusicService(
        IEnumerable<IMusicDownloadService> downloadServices,
        IFileSender fileSender,
        IMusicRepository musicRepository
        )
    {
        _downloadServices = downloadServices;
        _fileSender = fileSender;
        _musicRepository = musicRepository;
    }
    
    public async Task<string?> DownloadMusicAsync(Guid id)
    {
        var existsMusic = await _musicRepository.ExistsMusicByIdAsync(id);

        if (!existsMusic)
        {
            return null;
        }

        var downloader = _downloadServices.First();

        var music = await _musicRepository.GetMusicByIdAsync(id);

        var url = (await downloader.DownloadMusicsAsync([music])).First();

        return url;
    }
    
    public async Task<byte[]?> DownloadMusicsAsync(IEnumerable<Guid> ids)
    {
        List<string> musicUrls = new List<string>();
        
        foreach (var id in ids)
        {
            var t = await DownloadMusicAsync(id);
            if (t == null)
            {
                return null;
            }
            else
            {
                musicUrls.Add(t);
            }
        }
        
        var zipPath =  @"C:\Users\Werty\source\repos\Code\C#\WertyMusic\Presentation\bin\Debug\net9.0\Storage\Source_A\1.zip";
            
        CreateZipFromFileList(musicUrls,zipPath);

        var fileBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
        
        System.IO.File.Delete(zipPath);
        
        return fileBytes;
    }

    public async Task<IEnumerable<Music>> FindMusicsAsync(string sourceMusicName)
    {
        var existsMusic = await _musicRepository.ExistsMusicBySourceNameAsync(sourceMusicName);

        if (existsMusic != null)
        {
            return [existsMusic];
        }
        
        // must more strong logics (compare music from three different source site)
        List<Music> musics = new List<Music>();
        
        foreach (var downloadService in _downloadServices)
        {
            var downloaded = await downloadService.FindMusicsAsync(sourceMusicName);
            musics.AddRange(downloaded);
        }

        foreach (var music in musics)
        {
            await _musicRepository.AddMusicAsync(music);
        }
        
        return musics;
    }

    // what param really need?
    public async Task<Stream?> GetFileMusicAsync(Guid id)
    {
        var music = await _musicRepository.GetMusicByIdAsync(id);

        if (music == null)
            return null;
        
        return await _fileSender.GetFileAsync(music);
    }

    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        return await _musicRepository.GetMusicByIdAsync(id);
    }

    public async Task<IEnumerable<Music>?> GetMusicsAsync()
    {
        return await _musicRepository.GetMusicsAsync();
    }

    public async Task<Music?> AddMusicAsync(Music music)
    {
        return await _musicRepository.AddMusicAsync(music);
    }

    public async Task<Music?> UpDateMusicAsync(Guid id, Music music)
    {
        return await _musicRepository.UpdateMusicAsync(id, music);
    }

    private static void CreateZipFromFileList(IEnumerable<string> filePaths, string zipPath)
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