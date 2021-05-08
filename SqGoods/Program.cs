using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqGoods.DomainLogic.DataAccess;

namespace SqGoods
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var dbManager = host.Services.GetService<IDatabaseManager>()!;

            var err = await dbManager.Initialize();

            if (err != null)
            {
                var loggerFactory = host.Services.GetService<ILoggerFactory>() 
                    ?? throw new NullReferenceException("Could not find any logger factory");

                var logger = loggerFactory.CreateLogger("Startup");

                if (err.Exception != null)
                {
                    logger.LogError(err.Exception, err.CreateMessage());
                }
                else
                {
                    logger.LogError(err.CreateMessage());
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
