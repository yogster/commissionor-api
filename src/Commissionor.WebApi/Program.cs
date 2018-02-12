using Commissionor.WebApi.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Commissionor.WebApi
{
    public class Program
    {
        /// <summary>
        /// https://github.com/aspnet/Announcements/issues/258
        /// </summary>
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .Migrate()
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    /// <summary>
    /// https://stackoverflow.com/questions/45941707/why-remove-migration-run-my-app/45942026#45942026
    /// </summary>
    static class EntityFrameworkCoreExtensions
    {
        public static IWebHost Migrate(this IWebHost webhost)
        {
            using (var scope = webhost.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<CommissionorDbContext>())
                {

                    dbContext.Database.Migrate();
                }
            }
            return webhost;
        }
    }
}
