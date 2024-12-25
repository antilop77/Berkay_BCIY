using Common;
using Common.Model;
using Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workerServiceCoinAlSat.BackgroundTasks
{
    public class Step2AcikEmirDurumTakipWorkerService : BackgroundService, IDisposable
    {
        private readonly ILogger<Step2AcikEmirDurumTakipWorkerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DbHelper _dbHelper;
        private TimeSpan _period = TimeSpan.FromSeconds(6);
        private CoinsContext _dbContext;

        public Step2AcikEmirDurumTakipWorkerService(IConfiguration configuration, ILogger<Step2AcikEmirDurumTakipWorkerService> logger, CoinsContext dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbHelper = new DbHelper();
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await GetEmirDurumTakip();
                }
                catch (Exception e)
                {
                    Log.Error("Step2AcikEmirDurumTakip Global Error : " + e.Message);
                }
            }
        }

        private async Task<int> GetEmirDurumTakip()
        {
            var yeniEmirler = await _dbContext.Processes.Where(x => new[] { "PENDING" }.Contains(x.Status)).ToListAsync();
            int i = 0;
            foreach (var emir in yeniEmirler)
            {
                try
                {
                    var emirdurum = await BinanceHelper.GetOrderDetail(emir.Symbol!.Replace(".P", ""), emir.BinanceOrderId!);
                    await _dbHelper.ProcessLogYazAsync(emir.Id, emirdurum.ReqPath, emirdurum.ReqBody, emirdurum.ResBody, "GetEmirDurumTakip");

                    if (emirdurum.IsSuccess)
                    {
                        var status = emirdurum.Result.status;
                        if (status.Value == "FILLED")
                        {
                            emir.BinanceStatus = emirdurum.Result.status;
                            emir.Status = "OPENED";
                            emir.OpenedPrice = emirdurum.Result.avgPrice;
                            emir.UpdateDatetime = DateTime.Now;
                            emir.ExecutedQuantity = emirdurum.Result.executedQty;
                            await _dbContext.SaveChangesAsync();

                            Log.Information("Emir Durum No: " + emir.Id + "-" + emir.Symbol.ToString() + '-' + "OPENED");
                        }
                        else
                        {
                            string errorJson = emirdurum.ResBody;
                            if (errorJson != null)
                            {
                                BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
                                Log.Error($"Binance API Error: {error}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("GetEmirDurumTakip Hata " + e.Message);
                }

                i++;
            }
            return i;
        }
    }
}
