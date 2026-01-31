using System.IO.Compression;
using Domain.Interfaces.File;
using Domain.Models;
using Infrastructure.Options;

namespace Infrastructure.Services.Files;
using Microsoft.Extensions.Options;

public class ZipCreator: IZipCreator
{
    private readonly StorageOptions _storageOptions;
    public ZipCreator(IOptions<StorageOptions> options)
    {
        _storageOptions = options.Value;
    }
    
    public async Task<byte[]> CreateZipFromFileListAsync(IEnumerable<Music> musics)
    {
        var zipPath = Path.Combine(_storageOptions.LocalStorage ,Guid.NewGuid().ToString() + ".zip");
        
        using (FileStream zipStream = new FileStream(zipPath, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            foreach (var music in musics)
            {
                if (System.IO.File.Exists(music.Url))
                {
                    string fileName = $"{music.MusicName}.mp3";
                    archive.CreateEntryFromFile(music.Url, fileName);
                }
            }
        }
        
        var fileBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
        
        System.IO.File.Delete(zipPath);
        
        return fileBytes;
    }
}