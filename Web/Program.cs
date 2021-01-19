using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.Data;

namespace Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            await SeedInitialDataAsync(host);

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task SeedInitialDataAsync(IHost host)
        {
            using IServiceScope scope = host.Services.CreateScope();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILogger<Program>>();

            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            try
            {
                await context.SeedAsync(logger);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "error while seeding initial data");
            }
        }
    }
}
