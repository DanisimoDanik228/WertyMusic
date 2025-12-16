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
using PuppeteerSharp;

namespace Infrastructure.Services.DownloadServices;

public class SefonFindMusic : BaseMusicFind, IMusicFindService
{    
    private IBrowser? _browser;
    protected override int _maxCountSongForSearchSong { get; }
    
    
    public SefonFindMusic(IOptions<StorageOptions> options) : base(options)
    {        
        this._maxCountSongForSearchSong = 5;
    }
    
    private static string CreateUrlForSearch(string inputName)
    {
        return $"https://sefon.pro/search/{inputName.Replace(" ", "%20")}";
    }
    private static async Task<string> FindMusicDownloadUrl(string url)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");

        string downloadUrl;
        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {
            driver.Navigate().GoToUrl(url);

            string htmlContent = driver.PageSource;

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var downloadEl = document.DocumentNode.SelectSingleNode("//a[contains(@class, 'b_btn') and contains(@class, 'download') and contains(@class, 'no-ajix') and contains(@href, '/api/')]");
            downloadUrl = "https://sefon.pro" + downloadEl?.GetAttributeValue("href", "") ?? "";
        }
        
        return downloadUrl;
    }
    public async Task<IEnumerable<Music>> FindMusicsAsync(string musicName)
    {
        List<Music> res = new();
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless"); 
        
        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {        
            driver.Navigate().GoToUrl(CreateUrlForSearch(musicName));
            
            string htmlContent = driver.PageSource;
            
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            
            
            var mainSection = document.DocumentNode.SelectSingleNode("//div[@class='main']");
            var songs = mainSection.SelectNodes(".//div[@class='mp3']");


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
                
                res.Add(music);
            }
        }
        
        return res;
    }
}