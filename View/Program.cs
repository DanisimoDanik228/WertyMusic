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
using OpenQA.Selenium.Chrome;
using WertyMusic.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false) 
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

app.UseCustomMiddlewares();
app.UseStorageDirectory();
app.UseCustomRouting();

if (Environment.GetEnvironmentVariable("RUN_MIGRATIONS") == "true")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;
    while (retries-- > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("Database migrated");
            break;
        }
        catch
        {
            Console.WriteLine("Postgres not ready, retrying...");
            Thread.Sleep(3000);
        }
    }
}


app.Run();