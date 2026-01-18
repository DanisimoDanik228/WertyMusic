using Infrastructure.DBContext;
using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WertyMusic.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCustomStorageDirectory(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<StorageOptions>>();
        var directoryPath = options.Value.LocalStorage;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        Console.WriteLine("Storage directory is: " + directoryPath);

        return app;
    }

    public static WebApplication UseCustomRouting(this WebApplication app)
    {
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=BasiMusic}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        app.MapRazorPages();

        return app;
    }
    
    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        
        return app;
    }
    
    public static WebApplication RunCustomMigration(this WebApplication app)
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
        
        return app;
    }
}