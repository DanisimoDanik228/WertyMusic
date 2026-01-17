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
using WertyMusic.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false) 
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();
if (true)
{
    app.UseSwagger();  
    app.UseSwaggerUI(); 
}

app.UseCustomMiddlewares();
app.UseStorageDirectory();
app.UseCustomRouting();
if (Environment.GetEnvironmentVariable("RUN_MIGRATIONS") == "true")
{
    app.RunMigration();
}

app.Run();