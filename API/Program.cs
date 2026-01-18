using System.Reflection;
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
using WertyMusic;
using WertyMusic.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false) 
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddCustomSwagger(builder.Configuration);
builder.Services.AddCustomStorageDirectory(builder.Configuration);


var app = builder.Build();

app.UseCustomSwagger();
app.UseCustomMiddlewares();
app.UseCustomStorageDirectory();
app.UseCustomRouting();
if (Environment.GetEnvironmentVariable("RUN_MIGRATIONS") == "true")
{
    app.RunCustomMigration();
}

app.Run();