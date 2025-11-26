using System.Net;
using Domain.Enum;
using Domain.Interfaces;
using Domain.Interfaces.DownloadServices;
using Domain.Models;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;

namespace Infrastructure.Services.DownloadServices;

public class SiteADownloadService : BaseSongDowloader, IMusicDownloadService
{
    public SiteADownloadService(IOptions<StorageOptions> options) : base(options)
    {
        this.storageFolder = Path.Combine(_storageOptions.LocalStorage,"Source_A");
        this._maxCountSongForSearchSong = 5;
    }

    protected override string storageFolder { get; }
    protected override int _maxCountSongForSearchSong { get; }
    public async Task<string?> DownloadMusicAsync(Music music)
    {
        return DownloadMusic(music,storageFolder);
    }

    public async Task<IEnumerable<string>?> DownloadMusicsAsync(List<Music> music)
    {
        List<string> musicUrls = new List<string>();

        for (int i = 0; i < music.Count; i++)
        {
            var path = await DownloadMusicAsync(music[i]);
            
            musicUrls.Add(path);
            music[i].Url = path;
        }

        return musicUrls;
    }

    private static string DownloadMusic(Music info, string destinationDowloadFolder)
    {
        using (var client = new WebClient())
        {
            var filename = $"{info.MusicName} - {info.ArtistName}";
            var fullPath = Path.Combine(destinationDowloadFolder, filename) + ".mp3";
            client.DownloadFile(info.DownloadUrl, fullPath);

            return fullPath;
        } 
    }
    
    public async Task<IEnumerable<Music>> FindMusicsAsync(string musicName)
    {
        // must set the value of SourceName
        var musics = GetInfoSong(musicName);

        return musics;
    }

    private static Music FindApi(string url)
    {
        var driver = SetupDriver();
        try
        {
            driver.Navigate().GoToUrl(url);
            Music info = new();

            var h1 = driver.FindElement(By.TagName("h1"));
            string[] parts = SanitizeFileName(h1.Text).Trim().Split(new[] { " - " }, 2, StringSplitOptions.None);
            info.ArtistName = parts[0].Trim();
            info.MusicName = parts.Length > 1 ? parts[1].Trim() : "";
            //info.ArtistName = h1.FindElement(By.CssSelector("a")).GetAttribute("href");
            info.DownloadUrl = driver.FindElement(By.CssSelector("a.b_btn.download.no-ajix[href*='/api/']")).GetAttribute("href");
            
            return info;
        }
        finally
        {
            driver.Quit();
        }
    }
    
    private List<Music> GetInfoSong(string inputName)
    {
        var driver = SetupDriver();
        try
        {
            List<Music> res = new();
            driver.Navigate().GoToUrl(CreateUrlForSearch(inputName));

            var mainSection = driver.FindElements(By.CssSelector("div.main"));

            if (mainSection.Count == 0)
                return [];

            var songPages = mainSection[0].FindElements(By.CssSelector("a[href*='/mp3/']"));

            for (int i = 0; i < songPages.Count() && i < 2 * _maxCountSongForSearchSong; i += 2)
            {
                var t = FindApi(songPages[i].GetAttribute("href"));
                res.Add(t);
            }

            return res;
        }
        finally
        {
            driver.Quit();
        }
    }

    protected override string CreateUrlForSearch(string inputName)
    {
        return $"https://sefon.pro/search/{inputName.Replace(" ", "%20")}";
    }
}