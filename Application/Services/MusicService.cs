using System.IO.Compression;
using System.Runtime.ExceptionServices;
using ClassLibrary1.Interfaces;
using Domain.Interfaces;
using Domain.Interfaces.DownloadServices;
using Domain.Interfaces.File;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Repository.UnitOfWork;
using Domain.Models;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ClassLibrary1.Services;

public class MusicService : IMusicService
{
    private readonly IEnumerable<IMusicFindService> _downloadServices;
    private readonly IFileSender _fileSender;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StorageOptions _storageOptions;
    private readonly IDownloaderService _downloaderService;
    private readonly IZipCreator _zipCreator;
    public MusicService(
        IEnumerable<IMusicFindService> downloadServices,
        IFileSender fileSender,
        IUnitOfWork unitOfWork,
        IOptions<StorageOptions> options,
        IDownloaderService downloaderService,
        IZipCreator zipCreator
        )
    {
        _downloadServices = downloadServices;
        _fileSender = fileSender;
        _unitOfWork = unitOfWork;
        _storageOptions = options.Value;
        _downloaderService = downloaderService;
        _zipCreator = zipCreator;
    }
    
    public async Task<byte[]?> DownloadMusicsAsync(IEnumerable<Guid> ids)
    {
        var musics = await _unitOfWork.Music.GetMusicsByIdsAsync(ids);
        
        foreach (var music in musics)
        {
            if (music.Url == null)
            {
                var url = await _downloaderService.DownloadMusicAsync(music,_storageOptions.LocalStorage);
                music.Url = url;
            }
        }

        await _unitOfWork.SaveChangesAsync();
        
        var fileBytes = await _zipCreator.CreateZipFromFileListAsync(musics);
        
        return fileBytes;
    }

    public async Task<IEnumerable<Music>> FindMusicsAsync(string sourceMusicName)
    {
        var existsMusic = await _unitOfWork.Music.GetMusicsBySourceNameAsync(sourceMusicName);

        if (existsMusic != null && existsMusic.Count() > 0)
        {
            return existsMusic;
        }
        
        List<Music> musics = new List<Music>();
        
        foreach (var downloadService in _downloadServices)
        {
            var downloaded = await downloadService.FindMusicsAsync(sourceMusicName);
            musics.AddRange(downloaded);
        }

        await _unitOfWork.Music.AddMusicRangeAsync(musics);

        await _unitOfWork.SaveChangesAsync();
        
        return musics;
    }
    
    public async Task<Stream?> GetFileMusicAsync(Guid id)
    {
        var music = await _unitOfWork.Music.GetMusicByIdAsync(id);

        if (music == null)
            return null;
        
        return await _fileSender.GetFileAsync(music);
    }

    public async Task<IEnumerable<Music>?> GetMusicsAsync()
    {
        return await _unitOfWork.Music.GetMusicsAsync();
    }
}