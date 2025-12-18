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

namespace WertyMusic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        services.AddRazorPages();
        services.AddControllers();
        services.AddControllersWithViews();
        services.AddHttpClient();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IMusicRepository, MusicDBRepository>();
        
        services.Configure<StorageOptions>(configuration.GetSection("StorageOptions"));
        
        services.AddScoped<IFileSender, FileSender>();
        
        services.AddScoped<IMusicService, MusicService>();
        
        services.AddScoped<IDownloaderService, MusicDownloader>();
        
        services.AddScoped<IMusicFindService, HitmoFindMusic>();  
        services.AddScoped<IMusicFindService, SefonFindMusic>();

        return services;
    }
}