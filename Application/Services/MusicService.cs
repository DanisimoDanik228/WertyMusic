using System.IO.Compression;
using System.Runtime.ExceptionServices;
using ClassLibrary1.Interfaces;
using Domain.Interfaces;
using Domain.Interfaces.DownloadServices;
using Domain.Interfaces.File;
using Domain.Interfaces.Repository;
using Domain.Models;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ClassLibrary1.Services;

public class MusicService : IMusicService
{
    private readonly IEnumerable<IMusicDownloadService> _downloadServices;
    private readonly IFileSender _fileSender;
    private readonly IMusicRepository _musicRepository;
    private readonly StorageOptions _storageOptions;
    public MusicService(
        IEnumerable<IMusicDownloadService> downloadServices,
        IFileSender fileSender,
        IMusicRepository musicRepository,
        IOptions<StorageOptions> options
        )
    {
        _downloadServices = downloadServices;
        _fileSender = fileSender;
        _musicRepository = musicRepository;
        _storageOptions = options.Value;
    }
    
    public async Task<string?> DownloadMusicAsync(Guid id)
    {
        var existsMusic = await _musicRepository.GetMusicByIdAsync(id);

        if (existsMusic == null)
        {
            return null;
        }

        if (existsMusic.Url != null)
        {
            return existsMusic.Url;
        }

        var downloader = _downloadServices.First();

        var music = await _musicRepository.GetMusicByIdAsync(id);

        var url = await downloader.DownloadMusicAsync(music);
        music.Url = url;
        await _musicRepository.UpdateMusicAsync(id,music);
        
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
                var music = await _musicRepository.GetMusicByIdAsync(id);
                music.Url = t;
                await _musicRepository.UpdateMusicAsync(music.Id,music);
                musicUrls.Add(t);
            }
        }
        
        var zipPath =  Path.Combine(_storageOptions.LocalStorage ,Guid.NewGuid().ToString() + ".zip");
            
        CreateZipFromFileList(musicUrls,zipPath);

        var fileBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
        
        System.IO.File.Delete(zipPath);
        
        return fileBytes;
    }

    public async Task<IEnumerable<Music>> FindMusicsAsync(string sourceMusicName)
    {
        var existsMusic = await _musicRepository.GetMusicsBySourceNameAsync(sourceMusicName);

        if (existsMusic != null && existsMusic.Count() > 0)
        {
            return existsMusic;
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
        music.Id = Guid.NewGuid();
        
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
                }
            }
        }
    }
}