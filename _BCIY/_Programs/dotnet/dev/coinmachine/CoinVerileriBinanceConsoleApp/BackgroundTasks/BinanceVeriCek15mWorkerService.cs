using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Cronos;
using System.Security.AccessControl;
using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoinVerileriBinanceConsoleApp.BackgroundTasks
{
    public class BinanceVeriCek15mWorkerService : BackgroundService, IDisposable
    {
        private readonly ILogger<BinanceVeriCek15mWorkerService> _logger;
        private CoinsContext _dbContext;
        private readonly DbHelper _dbHelper;
        private Timer _timer;
        private TimeSpan _interval = TimeSpan.FromMinutes(15);

        private int islemSay = 0;

        public BinanceVeriCek15mWorkerService(ILogger<BinanceVeriCek15mWorkerService> logger, CoinsContext coinsContext)
        {
            _logger = logger;
            _dbContext = coinsContext;
            _dbHelper = new DbHelper();
            _timer = new Timer(ExecuteTimerCallback, null, TimeSpan.Zero, _interval);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExecuteTimerCallback, null, TimeSpan.Zero, _interval);
            return base.StartAsync(cancellationToken);
        }

        private void ExecuteTimerCallback(object state)
        {
            // ExecuteAsync metodunu çağırır
            ExecuteAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Burada yapılacak işlemler
                await Task.Run(() => VeriAl());
                await Task.Delay(_interval, stoppingToken);
            }
        }

        public void ChangeInterval(TimeSpan newInterval)
        {
            _interval = newInterval;
            _timer.Change(TimeSpan.Zero, _interval);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }

        private void VeriAl()
        {
            if (islemSay <= 0)
            {
                // TODO : BURDA KONTROL YAPILMALI
            }
            islemSay++;

            //var aktifCoinler = dbContext.CoinListesis.Where(x=>x.AktifDurumu==1).ToList();
            var aktifCoinler = _dbHelper.GetAllActiveCoinList();

            Stopwatch stopWatch = Stopwatch.StartNew();
            Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:fff"));
            foreach (var item in aktifCoinler)
            {
                Task.Run(() => BinanceHelper.GetBinanceMumVeri(item.CoinKodu!.Replace(".P", ""), "15m"));

                //var binanceVeri = BinanceHelper.GetBinanceMumVeri(item.CoinKodu!.Replace(".P", ""), "15m").Result;

                //Console.WriteLine("Calisti: " + item.CoinKodu + "-15m-" + DateTime.Now.ToString()+"-"+ binanceVeri.high);
                //Task.Run(() => getcoin(item));
            }

            stopWatch.Stop();
            TimeSpan timespan = stopWatch.Elapsed;

            DateTime SonVeriGelenVeriZamani = _dbHelper.CoinVerileriBinanceSonIslemSaati();

            var fark = SonVeriGelenVeriZamani.AddMinutes(15) - SonVeriGelenVeriZamani;

            double dakika = fark.TotalMinutes;

            ChangeInterval(fark);

        }
    }
}
