using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.ORM
{
    public static class MigrationsExtension
    {
        public static async Task<IApplicationBuilder> ApplyMigrationsAsync(this IApplicationBuilder app, CancellationToken cancellationToken = default)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            await context.Database.MigrateAsync(cancellationToken);
            return app;
        }
    }
}
