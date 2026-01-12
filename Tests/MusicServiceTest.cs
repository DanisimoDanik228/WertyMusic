using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Interfaces.DownloadServices;
using Domain.Interfaces.File;
using Domain.Interfaces.Repository;
using Infrastructure.DBContext;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.Services.DownloadService;
using Infrastructure.Services.DownloadServices;
using Infrastructure.Services.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Tests;

public class MusicServiceTest
{
    private readonly IMusicService _musicService;
    
    public MusicServiceTest()
    {
        StorageOptions storageOptions = new  StorageOptions(){LocalStorage = "TestStorage"};
        var optionsStorage = Options.Create(storageOptions);
        
        IEnumerable<IMusicFindService> downloadServices = [new HitmoFindMusic(optionsStorage),new SefonFindMusic(optionsStorage)];
        IFileSender fileSender = new FileSender();
        IDownloaderService downloaderService = new MusicDownloader();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;
        var dbContext = new AppDbContext(options);
        
        IMusicRepository musicRepository = new MusicDBRepository(dbContext);

        _musicService = new MusicService(
            downloadServices,
            fileSender,
            musicRepository,
            optionsStorage,
            downloaderService
        );
    }
    
    [Fact]
    public async Task Test_FindMusicsAsync_Access()
    {
        var songName = "eminem";
        
        var res = await _musicService.FindMusicsAsync(songName);
        
        Assert.NotNull(res);
        // count services(2) * count music in one(5) = 10
        Assert.Equal(10,res.Count());
    }
    
    [Fact]
    public async Task Test_FindMusicsAsync_Reject()
    {
        var songName = "bnmsdfusdbfsdlfhbfhsdkfsldfgbsdklfk";
        
        var res = await _musicService.FindMusicsAsync(songName);
        
        Assert.NotNull(res);
        Assert.Empty(res);
    }

    [Fact]
    public async Task Test_DownloadMusicsAsync_Reject()
    {
        var list = await  _musicService.FindMusicsAsync("eminem");
        
        List<Guid> musicIds = [list.First().Id];

        var res = await  _musicService.DownloadMusicsAsync(musicIds);
        
        Assert.NotNull(res);
    }
}