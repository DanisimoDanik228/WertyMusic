using System.IO.Compression;
using Domain.Interfaces.File;
using Domain.Models;

namespace Infrastructure.Services.Files;

public class ZipCreator: IZipCreator
{
    public void CreateZipFromFileList(IEnumerable<Music> musics, string zipPath)
    {
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
    }
}