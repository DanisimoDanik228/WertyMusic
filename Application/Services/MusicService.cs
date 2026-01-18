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
    private readonly IEnumerable<IMusicFindService> _downloadServices;
    private readonly IFileSender _fileSender;
    private readonly IMusicRepository _musicRepository;
    private readonly StorageOptions _storageOptions;
    private readonly IDownloaderService _downloaderService;
    private readonly IZipCreator _zipCreator;
    public MusicService(
        IEnumerable<IMusicFindService> downloadServices,
        IFileSender fileSender,
        IMusicRepository musicRepository,
        IOptions<StorageOptions> options,
        IDownloaderService downloaderService,
        IZipCreator zipCreator
        )
    {
        _downloadServices = downloadServices;
        _fileSender = fileSender;
        _musicRepository = musicRepository;
        _storageOptions = options.Value;
        _downloaderService = downloaderService;
        _zipCreator = zipCreator;
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

        var url = await _downloaderService.DownloadMusicAsync(existsMusic,_storageOptions.LocalStorage);
        if (url == null)
        {
            return null;
        }
        await _musicRepository.UpdateMusicUrlAsync(id,url);
        
        return url;
    }
    
    public async Task<byte[]?> DownloadMusicsAsync(IEnumerable<Guid> ids)
    {
        List<Music> musics = new();
        
        foreach (var id in ids)
        {
            var url = await DownloadMusicAsync(id);
            if (url != null)
            {
                var music = await _musicRepository.UpdateMusicUrlAsync(id,url);
                musics.Add(music);
            }
        }
        
        var zipPath =  Path.Combine(_storageOptions.LocalStorage ,Guid.NewGuid().ToString() + ".zip");
            
        _zipCreator.CreateZipFromFileList(musics,zipPath);

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
}