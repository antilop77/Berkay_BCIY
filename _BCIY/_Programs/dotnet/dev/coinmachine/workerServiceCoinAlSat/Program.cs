using Common;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;
using System;
using workerServiceCoinAlSat.BackgroundTasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Binance.Net;

namespace workerServiceCoinAlSat
{
    public class Program
    {
        public static void Main(string[] args)
        {

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices( (hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    AppSettings.Configuration = configuration;
                    AppSettings.ConnectionString = configuration.GetConnectionString("sql_coin");
                    AppSettings.apiKey = configuration.GetSection("AppSettings:apiKey").Value!;
                    AppSettings.secretKey = configuration.GetSection("AppSettings:secretKey").Value!;
                    AppSettings.endPoint = configuration.GetSection("AppSettings:endPoint").Value!;

                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();


                    var optionBuilder = new DbContextOptionsBuilder<CoinsContext>();
                    optionBuilder.UseSqlServer(AppSettings.ConnectionString);
                    services.AddTransient<CoinsContext>(d => new CoinsContext(optionBuilder.Options));

                    services.AddBinance(options => {
                        // Options can be configured here, for example:
                        options.RateLimiterEnabled = true;
                        options.RateLimitingBehaviour = RateLimitingBehaviour.Wait;
                        options.ApiCredentials = new ApiCredentials(AppSettings.apiKey, AppSettings.secretKey);
                    });
                    BinanceExchange.RateLimiter.RateLimitTriggered += (rateLimitEvent) => Console.WriteLine("Limit triggered: " + rateLimitEvent);



                    services.AddHostedService<WorkerTest>();
                    // services.AddHostedService<AlSat1SaniyeWorkerService>();

                    //services.AddHostedService<Step1YeniEmirAcWorkerService>();
                    //services.AddHostedService<Step2AcikEmirDurumTakipWorkerService>();
                    //services.AddHostedService<Step3TersEmirAcWorkerService>();
                    //services.AddHostedService<Step4PozisyonKontrolWorkerService>();
                    //services.AddHostedService<IsoleMarginAyarlaWorkerService>();
                })
            .Build();

            

            host.Run();
        }
    }
}