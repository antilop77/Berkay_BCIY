using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workerServiceCoinAlSat.BackgroundTasks
{
    public class IsoleMarginAyarlaWorkerService : BackgroundService, IDisposable
    {
        private readonly ILogger<IsoleMarginAyarlaWorkerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DbHelper _dbHelper;
        private TimeSpan _period = TimeSpan.FromHours(1);

        public IsoleMarginAyarlaWorkerService(IConfiguration configuration, ILogger<IsoleMarginAyarlaWorkerService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _dbHelper = new DbHelper();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    await MarginTipleriniGuncelle();
                    await LeverageGuncelle();
                    await Task.Delay(_period, stoppingToken);
                }
                catch (Exception e)
                {
                    Log.Error("PozisyonKontrolWorkerService Global Error :" + e.Message);
                }
            }
        }

        private async Task<int> MarginTipleriniGuncelle()
        {
            var aktifCoinler = _dbHelper.CoinListesiGetir();
            List<Task> tasks = new List<Task>();

            foreach (var item in aktifCoinler)
            {
                Task task = Task.Run(async () =>
                {

                    try
                    {
                        await BinanceHelper.marginTypeDegistir(item.CoinKodu.Replace(".P", ""));
                        Log.Information("Margin Tip guncellendi:" + item.CoinKodu.Replace(".P", ""));
                    }
                    catch (Exception e)
                    {
                        Log.Error("MarginTipleriniGuncelle Hata:" + item.CoinKodu.Replace(".P", "") + e.Message);
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return 1;
        }

        private async Task<int> LeverageGuncelle()
        {
            var aktifCoinler = _dbHelper.CoinListesiGetir();

            foreach (var item in aktifCoinler)
            {
                try
                {
                    await BinanceHelper.LeverageDegistir(item.CoinKodu.Replace(".P", ""), "10");
                    Log.Information("Leverage Guncellendi:" + item.CoinKodu.Replace(".P", ""));
                }
                catch (Exception e)
                {
                    Log.Error("LeverageGuncelle Hata:" + item.CoinKodu.Replace(".P", "")+"-"+e.Message);
                }
            }
            return 1;
        }

    }
}
