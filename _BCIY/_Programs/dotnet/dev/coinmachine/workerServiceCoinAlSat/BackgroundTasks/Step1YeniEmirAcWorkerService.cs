using Common;
using Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workerServiceCoinAlSat.BackgroundTasks
{
    public class Step1YeniEmirAcWorkerService : BackgroundService, IDisposable
    {
        private readonly ILogger<Step1YeniEmirAcWorkerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DbHelper _dbHelper;
        private TimeSpan _period = TimeSpan.FromSeconds(2);
        private CoinsContext _dbContext;
        private bool sistemONOFF = false;

        public Step1YeniEmirAcWorkerService(IConfiguration configuration, ILogger<Step1YeniEmirAcWorkerService> logger, CoinsContext dbContext)
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
                    await GetYeniEmirAc();
                }
                catch (Exception e)
                {
                    Log.Error("Step1YeniEmirAcWorkerService Global Error : " + e.Message);
                }
            }
        }

        private async Task<int> GetYeniEmirAc()
        {
            //var yeniEmirler = _dbContext.Processes.Where(x => x.Status == "NEW").ToList();
            var yeniEmirler = _dbContext.Processes.Where(x => new[] { "NEW" }.Contains(x.Status) && x.InsertDatetime >= DateTime.Now.AddMinutes(-3)).ToList();
            int i = 0;
            foreach (var emir in yeniEmirler)
            {
                try
                {
                    var tipdegistir = await BinanceHelper.marginTypeDegistir(emir.Symbol);
                    Log.Information("Yeni Emir Ac Emri Geldi:" + emir.Symbol.ToString());
                    var alemri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: emir.OrderSide, quantity: emir.Quantity.ToString().Replace(",", "."), amount: emir.OpenPrice.Value.ToString().Replace(",", "."), StopPrice: emir.StopPrice.Value.ToString().Replace(",", "."), type: emir.OrderType);
                    _dbHelper.ProcessLogYaz(emir.Id, alemri.ReqPath, alemri.ReqBody, alemri.ResBody, "YENI_EMIR_AC");

                    if (alemri.IsSuccess)
                    {
                        emir.BinanceOrderId = alemri.Result.orderId;
                        emir.Status = "PENDING";
                        emir.UpdateDatetime = DateTime.Now;
                        _dbContext.SaveChanges();

                        Log.Information("Yeni Emir Açıldı:" + emir.Symbol.ToString());
                    }
                    else
                    {
                        emir.Status = "ERROR";
                        emir.UpdateDatetime = DateTime.Now;
                        _dbContext.SaveChanges();

                        Log.Information("Yeni Emir Hata:" + emir.Symbol.ToString() + '-' + alemri.Message);
                    }

                    _dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Log.Error("GetYeniEmirAc Hata " + e.Message);
                }
                i++;
            }
            return i;
        }

    }
}
