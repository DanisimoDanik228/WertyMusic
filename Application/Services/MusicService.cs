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
    
    public async Task<IEnumerable<Music>> DownloadMusicsAsync(string musicName)
    {
        var existsMusic = await _musicRepository.ExistsMusicAsync(musicName);

        if (existsMusic != null)
        {
            return [existsMusic];
        }
        
        // must more strong logics (compare music from three different source site)
        List<Music> musics = new List<Music>();
        
        foreach (var downloadService in _downloadServices)
        {
            var downloaded = await downloadService.DownloadMusicsAsync(musicName);
            musics.AddRange(downloaded);
        }
        
        return musics;
    }

    public async Task<IEnumerable<Music>> FindMusicsAsync(string musicName)
    {
        var existsMusic = await _musicRepository.ExistsMusicAsync(musicName);

        if (existsMusic != null)
        {
            return [existsMusic];
        }
        
        // must more strong logics (compare music from three different source site)
        List<Music> musics = new List<Music>();
        
        foreach (var downloadService in _downloadServices)
        {
            var downloaded = await downloadService.FindMusicsAsync(musicName);
            musics.AddRange(downloaded);
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

    public async Task<Music?> GetMusicByNameAsync(string musicName)
    {
        return await _musicRepository.GetMusicsByNameAsync(musicName);
    }

    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        return await _musicRepository.GetMusicByIdAsync(id);
    }

    public async Task<Music?> AddMusicAsync(Music music)
    {
        return await _musicRepository.AddMusicAsync(music);
    }
}