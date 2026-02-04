using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using Domain.Enum;
using Domain.Interfaces.DownloadServices;
using Domain.Models;
using HtmlAgilityPack;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using PuppeteerSharp;

namespace Infrastructure.Services.DownloadServices;

public class SefonFindMusic : BaseMusicFind, IMusicFindService
{
    protected override int _maxCountSongForSearchSong { get; }

    public SefonFindMusic(IOptions<StorageOptions> storageOptions,IOptions<SeleniumOptions> selenuimOptions) 
        : base(storageOptions,selenuimOptions)    
    {        
        this._maxCountSongForSearchSong = 5;
    }
    
    private static string CreateUrlForSearch(string inputName)
    {
        return $"https://sefon.pro/search/{inputName.Replace(" ", "%20")}";
    }
    private async Task<string> FindMusicDownloadUrl(string url)
    {
        string htmlContent;
        using (IWebDriver driver = CreateDriver(_seleniumOptions))
        {
            driver.Navigate().GoToUrl(url);

            htmlContent = driver.PageSource;
        }

        var document = new HtmlDocument();
        document.LoadHtml(htmlContent);

        var downloadEl = document.DocumentNode.SelectSingleNode("//a[contains(@class, 'b_btn') and contains(@class, 'download') and contains(@class, 'no-ajix') and contains(@href, '/api/')]");
        string downloadUrl = "https://sefon.pro" + downloadEl?.GetAttributeValue("href", "") ?? "";
        
        return downloadUrl;
    }
    public async IAsyncEnumerable<Music> FindMusicsAsync(string musicName)
    {
        string htmlContent;
        using (IWebDriver driver = CreateDriver(_seleniumOptions))
        {
            driver.Navigate().GoToUrl(CreateUrlForSearch(musicName));

            htmlContent = driver.PageSource;
        }

        var document = new HtmlDocument();
        document.LoadHtml(htmlContent);
        
        
        var mainSection = document.DocumentNode.SelectSingleNode("//div[@class='main']");
        if (mainSection == null)
        {
            yield break;
        }
        
        var songs = mainSection.SelectNodes(".//div[@class='mp3']");
        if (songs == null)
        {
            yield break;
        }

        for (int i = 0; i < songs.Count && i < _maxCountSongForSearchSong; i++)
        {
            var songDiv = songs[i];
            
            var artistNameDiv = songDiv.SelectSingleNode(".//div[@class='artist_name']");
            var artistNameA = artistNameDiv?.SelectSingleNode(".//a");
            var artistName = artistNameA?.InnerText;

            var songNameDiv = songDiv.SelectSingleNode(".//div[@class='song_name']");
            var songNameA = songNameDiv?.SelectSingleNode(".//a");
            var songName = songNameA?.InnerText;
            
            var hrefMusic = songNameA?.GetAttributeValue("href", "");
            var musicUrl = "https://sefon.pro" + hrefMusic;
            var downloadUrl = await FindMusicDownloadUrl(musicUrl);
            
            var hrefArtist =  artistNameA?.GetAttributeValue("href", "");
            var artistUrl = "https://sefon.pro" + hrefArtist;

            var music = new Music();

            music.DownloadUrl = downloadUrl;
            music.ArtistUrl = artistUrl;
            music.ArtistName = artistName;
            music.MusicName = songName;
            music.SiteSource = SiteSource.A;
            music.SourceName = musicName;
            music.CreationDate = DateTime.UtcNow;
            
            yield return music;
        }
    }
}