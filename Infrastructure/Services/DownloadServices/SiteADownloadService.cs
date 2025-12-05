using System.Net;
using AngleSharp;
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
        this._storageFolder = Path.Combine(_storageOptions.LocalStorage,"Source_A");
        this._maxCountSongForSearchSong = 5;
    }

    protected override string _storageFolder { get; }
    protected override int _maxCountSongForSearchSong { get; }
    public async Task<string?> DownloadMusicAsync(Music music)
    {
        return DownloadMusic(music,_storageFolder);
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
        var musics = GetInfoSongAsync(musicName);

        return await musics;
    }

    private readonly HttpClient _httpClient = new HttpClient();
    private async Task<Music> FindApiAsync(string url)
    {
        var content = await _httpClient.GetStringAsync(url);

        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(content));

        Music info = new Music();

        var h1 = document.QuerySelector("h1");
        string h1Text = h1?.TextContent ?? "";
        string[] parts = SanitizeFileName(h1Text).Trim().Split(new[] { " - " }, 2, StringSplitOptions.None);
        info.ArtistName = parts[0].Trim();
        info.MusicName = parts.Length > 1 ? parts[1].Trim() : "";

        var downloadEl = document.QuerySelector("a.b_btn.download.no-ajix[href*='/api/']");
        info.DownloadUrl = downloadEl?.GetAttribute("href") ?? "";

        return info;
    }
    public async Task<List<Music>> GetInfoSongAsync(string inputName)
    {
        var url = CreateUrlForSearch(inputName);
        var content = await _httpClient.GetStringAsync(url);

        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(content));

        List<Music> res = new();

        var mainSection = document.QuerySelector("div.main");
        if (mainSection == null)
            return res;

        var songPages = mainSection.QuerySelectorAll("a[href*='/mp3/']");

        for (int i = 0; i < songPages.Length && i < 2 * _maxCountSongForSearchSong; i += 2)
        {
            var href = songPages[i].GetAttribute("href");
            if (string.IsNullOrWhiteSpace(href))
                continue;

            var info = await FindApiAsync("https://sefon.pro"+href);
            info.SiteSource = SiteSource.A;
            info.SourceName = inputName;
            info.CreationDate = DateTime.UtcNow;

            res.Add(info);
        }

        return res;
    }

    private static string CreateUrlForSearch(string inputName)
    {
        return $"https://sefon.pro/search/{inputName.Replace(" ", "%20")}";
    }
}