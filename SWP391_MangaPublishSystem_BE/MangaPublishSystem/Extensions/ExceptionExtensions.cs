using MangaPublishSystem.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace MangaPublishSystem.Extensions
{
    public static class ExceptionExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
