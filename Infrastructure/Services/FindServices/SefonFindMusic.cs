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
    private async Task<Music> FindApiAsync(string url)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");

        Music info = new Music();
        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {
            driver.Navigate().GoToUrl(url);

            string htmlContent = driver.PageSource;

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var h1 = document.DocumentNode.SelectSingleNode("//h1");
            string h1Text = h1?.InnerText ?? "";

    // Очищаем и разбиваем текст
            string[] parts = SanitizeFileName(h1Text).Trim().Split(new[] { " - " }, 2, StringSplitOptions.None);
            info.ArtistName = parts[0].Trim();
            info.MusicName = parts.Length > 1 ? parts[1].Trim() : "";

    // Ищем ссылку для скачивания (XPath с несколькими условиями)
            var downloadEl = document.DocumentNode.SelectSingleNode("//a[contains(@class, 'b_btn') and contains(@class, 'download') and contains(@class, 'no-ajix') and contains(@href, '/api/')]");
            info.DownloadUrl = "https://sefon.pro" + downloadEl?.GetAttributeValue("href", "") ?? "";
        }
        return info;
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
            var songPages = mainSection.SelectNodes(".//a[contains(@href, '/mp3/')]");
            
            for (int i = 0; i < songPages.Count && i < 2 * _maxCountSongForSearchSong; i += 2)
            {
                var href = songPages[i].GetAttributeValue("href", "");
                if (string.IsNullOrWhiteSpace(href))
                    continue;

                var info = await FindApiAsync("https://sefon.pro" + href);
                info.SiteSource = SiteSource.A;
                info.SourceName = musicName;
                info.CreationDate = DateTime.UtcNow;

                res.Add(info);
            }
        }
        
        return res;
    }
}