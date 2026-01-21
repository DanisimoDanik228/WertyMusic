using System.IO.Compression;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Interfaces.DownloadServices;
using Domain.Interfaces.File;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Repository.UnitOfWork;
using Infrastructure.DBContext;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.Repository.UnitOfWork;
using Infrastructure.Services.DownloadService;
using Infrastructure.Services.DownloadServices;
using Infrastructure.Services.Files;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Tests;

public class MusicServiceTest
{
    private readonly IMusicService _musicService;
    
    public MusicServiceTest()
    {
        StorageOptions storageOptions = new  StorageOptions(){LocalStorage = "TestStorage"};
        
        Directory.CreateDirectory(storageOptions.LocalStorage);
        
        SeleniumOptions selenuimOptions = new SeleniumOptions();
        var optionsStorage = Options.Create(storageOptions);
        var seleniumOptions = Options.Create(selenuimOptions);
        
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var dbContext = new AppDbContext(dbOptions);
        dbContext.Database.EnsureCreated();
        
        IEnumerable<IMusicFindService> downloadServices = [new HitmoFindMusic(optionsStorage,seleniumOptions),new SefonFindMusic(optionsStorage,seleniumOptions)];
        IFileSender fileSender = new FileSender();
        IDownloaderService downloaderService = new MusicDownloader();
        IMusicRepository musicRepository = new MusicDbRepository(dbContext);
        IUnitOfWork unitOfWork = new DbUnitOfWork(dbContext,musicRepository);
        IZipCreator zipCreator = new ZipCreator();
        
        _musicService = new MusicService(
            downloadServices,
            fileSender,
            unitOfWork,
            optionsStorage,
            downloaderService,
            zipCreator
        );
    }
    
    [Fact]
    public async Task Test_FindMusicsAsync_Access()
    {
        var songName = "eminem";
        
        var res = await _musicService.FindMusicsAsync(songName);
        
        Assert.NotNull(res);
        Assert.Equal(10,res.Count());
        
        
        songName = "kill";
        
        res = await _musicService.FindMusicsAsync(songName);
        
        Assert.NotNull(res);
        Assert.Equal(10,res.Count());


        res = await _musicService.GetMusicsAsync();
        
        Assert.NotNull(res);
        Assert.Equal(20,res.Count());
    }
    
    [Fact]
    public async Task Test_DownloadMusicsAsync_Access()
    {
        var songName = "eminem";
        
        var res = await _musicService.FindMusicsAsync(songName);
        var fileBytes = await _musicService.DownloadMusicsAsync(res.Select(r => r.Id));

        using (var stream = new MemoryStream(fileBytes))
        {
            using (var zip = new ZipArchive(stream))
            {
                int count = zip.Entries.Count;

                Assert.Equal(10,count);
                
                foreach (var entry in zip.Entries)
                {
                    Assert.True(entry.Length > 1_000_000);
                    Assert.EndsWith(".mp3",entry.Name);
                }
            }
        }
    }
    
    [Fact]
    public async Task Test_FindMusicsAsync_Reject()
    {
        var songName = "bnmsdfusdbfsdlfhbfhsdkfsldfgbsdklfk";
        
        var res = await _musicService.FindMusicsAsync(songName);
        
        Assert.NotNull(res);
        // count services(2) * count music in one(5) = 10
        Assert.Empty(res);
        
        
        res = await _musicService.GetMusicsAsync();
        
        Assert.NotNull(res);
        Assert.Empty(res);
    }
}