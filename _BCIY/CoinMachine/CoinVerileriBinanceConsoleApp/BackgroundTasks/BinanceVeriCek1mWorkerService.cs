using Common;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CoinVerileriBinanceConsoleApp.BackgroundTasks
{
    public class BinanceVeriCek1mWorkerService : BackgroundService, IDisposable
    {
        private readonly ILogger<BinanceVeriCek15mWorkerService> _logger;
        private CoinsContext _dbContext;
        private readonly DbHelper _dbHelper;
        //private Timer _timer;
        private TimeSpan _period = TimeSpan.FromSeconds(1);
        private bool _is15mDataProcessed = false;
        private bool _is1mDataProcessed = false;
        private bool _is1hDataProcessed = false;
        DateTime uygulamaZamani = DateTime.Now;

        public BinanceVeriCek1mWorkerService(ILogger<BinanceVeriCek15mWorkerService> logger, CoinsContext coinsContext, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;

            _dbContext = coinsContext;
            _dbHelper = new DbHelper();

            var currentTime = DateTime.Now;
            // Zaman dilimini belirle (örneğin, 15 dakikalık bir zaman dilimi)
            var timeInterval = 60;
            // Şu anki zamanı belirli zaman dilimine yuvarla
            var roundedTime = RoundToNearestPastInterval(currentTime, timeInterval);
            
        }

        private static DateTime RoundToNearestPastInterval(DateTime time, int interval)
        {
            var minutes = time.Minute;
            var remainder = minutes % interval;
            var roundedMinutes = minutes - remainder;

            // Eğer şu anki zaman verilen zaman aralığının tam katı ise, aynı saat ve dakikayı döndür
            if (remainder == 0)
            {
                return new DateTime(
                    time.Year,
                    time.Month,
                    time.Day,
                    time.Hour,
                    minutes,
                    0,
                    time.Kind
                );
            }

            // Değilse, en yakın geçmiş zaman dilimine yuvarla
            return new DateTime(
                time.Year,
                time.Month,
                time.Day,
                time.Hour,
                roundedMinutes,
                0,
                time.Kind
            );
        }

        private async Task<DateTime> binanceSaat()
        {
            try
            {
                using var client = new HttpClient();
                DateTime tarih = DateTime.Now;
                try
                {
                    // Binance API'sinden zamanı al
                    HttpResponseMessage response = await client.GetAsync("https://fapi.binance.com/fapi/v1/time");
                    response.EnsureSuccessStatusCode(); // İstek başarılı olmadıysa hata fırlat

                    // JSON yanıtını oku ve tarihi çıkar
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    long tarihUnix = result.serverTime;

                    // Unix zamanını .NET DateTime'e çevir
                    tarih = DateTimeOffset.FromUnixTimeMilliseconds(tarihUnix).DateTime;

                    // Tarihi yazdır
                    Console.WriteLine("Binance API'den alınan tarih: " + tarih);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"HTTP isteği başarısız Sistem Saati Aktif: {e.Message}");
                }
                return tarih;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            uygulamaZamani = await binanceSaat();

           // Task.Run(() => BinanceHelper.GetBinanceMumVeriGecmisYaz("AVAXUSDT", "15d", "96"));

            using PeriodicTimer timer = new PeriodicTimer(_period);
            uygulamaZamani.AddSeconds(_period.TotalSeconds);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                DateTime now = DateTime.Now;
                int seconds = now.Second;
                int minutes = now.Minute;


                //Her dakika bir iş yap
                //if (seconds == 0)
                //{
                //    // Her dakika bir iş yapılacak metodu buraya çağırın
                //    //VeriAl("1m");
                //    VeriAl("15m");
                //}

                // Her 15 dakikada bir iş yap
                if (minutes % 15 == 0 && seconds == 0)
                {
                    // Her 15 dakikada bir iş yapılacak metodu buraya çağırın
                    VeriAl("15m");
                }

                // Her saat bir iş yap
                if (minutes == 0 && seconds == 0)
                {
                    // Her saat bir iş yapılacak metodu buraya çağırın
                    VeriAl("1h");
                }

                if (seconds == 50) // saati guncelle
                {
                    uygulamaZamani = await binanceSaat();
                }

                //_logger.LogInformation("BinanceVeriCek1mWorkerService...");
            }
        }

        private void VeriAl(string mumBilgisi = "15m")
        {
            var aktifCoinler = _dbHelper.GetAllActiveCoinList();

            foreach (var item in aktifCoinler)
            {
                Task.Run(() => BinanceHelper.GetBinanceMumVeri(item.CoinKodu!.Replace(".P", ""), mumBilgisi));
            }
            //DateTime SonVeriGelenVeriZamani = _dbHelper.CoinVerileriBinanceSonIslemSaati();
            //var fark = SonVeriGelenVeriZamani.AddMinutes(15) - SonVeriGelenVeriZamani;
            //double dakika = fark.TotalMinutes;
        }
    }
}
