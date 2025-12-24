using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WertyMusic.Extensions;

public static class ExceptionHandling
{
    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        
        return app;
    }
}