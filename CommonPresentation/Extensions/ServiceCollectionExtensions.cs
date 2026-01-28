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
using System.Reflection;
using Domain.Interfaces.Repository.UnitOfWork;
using Domain.Models;
using Infrastructure.Repository.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace WertyMusic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddRazorPages();
        services.AddControllers();
        services.AddControllersWithViews();
        services.AddHttpClient();

        services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"));
        var dbSettings = configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();

        if (dbSettings == null)
        {
            throw new Exception("Database settings not found");
        }
        
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbSettings.Postgres.ConnectionString));

        services.AddSingleton(new MongoClient(dbSettings.MongoDb.ConnectionString).GetDatabase(dbSettings.MongoDb.DatabaseName));
        BsonClassMap.RegisterClassMap<Music>(cm =>
        {
            cm.AutoMap();
    
            cm.MapIdProperty(m => m.Id)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                .SetElementName("_id"); 
        });
        
        
        services.AddScoped<IMusicRepository, MusicMongoDbRepository>();
        
        services.Configure<SeleniumOptions>(configuration.GetSection("SeleniumOptions"));
        
        services.AddScoped<IFileSender, FileSender>();
        
        services.AddScoped<IMusicService, MusicService>();
        
        services.AddScoped<IDownloaderService, MusicDownloader>();
        
        services.AddScoped<IMusicFindService, HitmoFindMusic>();  
        services.AddScoped<IMusicFindService, SefonFindMusic>();
        
        services.AddScoped<IZipCreator, ZipCreator>();
        
        services.AddScoped<IUnitOfWork, UnitOfWorkMongoDb>();
        
        return services;
    }

    public static IServiceCollection AddCustomStorageDirectory(this IServiceCollection services, IConfiguration configuration)
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        services.Configure<StorageOptions>(configuration.GetSection("StorageOptions"));
        services.PostConfigure<StorageOptions>(options =>
        {
            if (!Path.IsPathRooted(options.LocalStorage))
            {
                options.LocalStorage = Path.Combine(localAppData, options.LocalStorage);
            }
        });
        
        return services;
    }
}