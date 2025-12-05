using System.Net;
using Domain.Interfaces.DownloadServices;
using Domain.Models;

namespace Infrastructure.Services.DownloadService;

public class MusicDownloader :IDownloaderService
{
    
    public async Task<IEnumerable<string>?> DownloadMusicsAsync(List<Music> music,string destination)
    {
        List<string> musicUrls = new List<string>();

        for (int i = 0; i < music.Count; i++)
        {
            var path = await DownloadMusicAsync(music[i],destination);
            
            musicUrls.Add(path);
            music[i].Url = path;
        }

        return musicUrls;
    }

    public async Task<string?> DownloadMusicAsync(Music music,string destination)
    {
        using (var client = new WebClient())
        {
            var filename = $"{music.MusicName} - {music.ArtistName}";
            var fullPath = Path.Combine(destination, filename) + ".mp3";
            
            client.DownloadFile(music.DownloadUrl, fullPath);

            return fullPath;
        } 
    }
}