using CoinVerileriBinanceConsoleApp.BackgroundTasks;
using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoinVerileriBinanceConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   IConfiguration configuration = hostContext.Configuration;

                   AppSettings.Configuration = configuration;
                   AppSettings.ConnectionString = configuration.GetConnectionString("sql_coin");

                   var optionBuilder = new DbContextOptionsBuilder<CoinsContext>();
                   optionBuilder.UseSqlServer(AppSettings.ConnectionString);
                   services.AddScoped<CoinsContext>(d => new CoinsContext(optionBuilder.Options));


                  // services.AddHostedService<BinanceVeriCek15mWorkerService>();
                   services.AddHostedService<BinanceVeriCek1mWorkerService>();
               })
           .Build();



            host.Run();

        }
    }
}
