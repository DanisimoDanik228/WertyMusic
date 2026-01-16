using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace WertyMusic.Extensions;

public static class MigrationBDExtension
{
    public static WebApplication RunMigration(this WebApplication app)
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