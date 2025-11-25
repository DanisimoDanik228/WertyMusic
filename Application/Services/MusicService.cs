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
    
    public async Task<IEnumerable<string>?> DownloadMusicsAsync(IEnumerable<Guid> ids)
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
        
        return musicUrls;
    }

    public async Task<IEnumerable<Music>> FindMusicsAsync(string sourceMusicName)
    {
        var existsMusic = await _musicRepository.ExistsMusicByNameAsync(sourceMusicName);

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
}