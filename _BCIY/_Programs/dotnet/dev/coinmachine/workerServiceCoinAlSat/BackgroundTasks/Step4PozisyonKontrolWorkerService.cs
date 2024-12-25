using Azure;
using Common;
using Common.Model;
using Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workerServiceCoinAlSat.BackgroundTasks
{
    public class Step4PozisyonKontrolWorkerService : BackgroundService, IDisposable
    {
        private readonly ILogger<Step4PozisyonKontrolWorkerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DbHelper _dbHelper;
        private TimeSpan _period = TimeSpan.FromSeconds(10);
        private CoinsContext _dbContext;

        public Step4PozisyonKontrolWorkerService(IConfiguration configuration, ILogger<Step4PozisyonKontrolWorkerService> logger, CoinsContext dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbHelper = new DbHelper();
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PozisyonKontrol();
                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception e)
                {
                    Log.Error("PozisyonKontrolWorkerService Global Error :" + e.Message);
                }
            }
        }

        private async Task<int> PozisyonKontrol()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //var yeniEmirler = await _dbContext.Processes.Where(x => new[] { "OPENED", "RENEW" }.Contains(x.Status)).ToListAsync();
            var yeniEmirler = await _dbHelper.GuncellenecekPozisyonKontrolListesi();
            int i = 0;

            await Task.WhenAll(yeniEmirler.Select(async emir =>
            {
                try
                {
                    //Log.Information("PozisyonKontrol Emir No :" + emir.Id.ToString() + "-" + emir.Symbol!.ToString());

                    string emirtipi = emir.OrderSide!;
                    string yeniemirTipi = string.Empty;

					if (emir.BinanceCloseOrderId != null)
					{
						var orderDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BinanceCloseOrderId);
						await _dbHelper.ProcessLogYazAsyncADO(emir.Id, orderDetail.ReqPath, orderDetail.ReqBody, orderDetail.ResBody, "PozisyonKontrol");

						if (orderDetail.IsSuccess)
						{
							var status = orderDetail.Result.status;

							if (status.Value == "FILLED")
							{
								if (emir.BinanceTrailingOrderId != null)
								{
									await BinanceHelper.OrderSil(emir.Symbol.Replace(".P", ""), emir.BinanceTrailingOrderId);
								}

								emir.Status = "CLOSED";
								emir.ClosedPrice = orderDetail.Result.stopPrice;
								emir.ClosedDatetime = DateTime.Now;

								await _dbHelper.CoinPozisyonDurumUpdate(emir);

								
								
								//await _dbContext.SaveChangesAsync();

								Log.Information("Emir Durum CLOSED : " + emir.Symbol.Replace(".P", "").ToString());
							}

							if (status.Value == "EXPIRED")
							{
								if (emir.BinanceTrailingOrderId != null)
								{
									await BinanceHelper.OrderSil(emir.Symbol.Replace(".P", ""), emir.BinanceTrailingOrderId);
								}

								emir.Status = "EXPIRED";
								emir.ClosePrice = orderDetail.Result.stopPrice;
								emir.ClosedDatetime = DateTime.Now;
								await _dbHelper.CoinPozisyonDurumUpdate(emir);
								//_dbContext.SaveChanges();

								Log.Information("Emir Durum EXPIRED :" + emir.Symbol.Replace(".P", "").ToString());
							}
						}
						else
						{
							string errorJson = orderDetail.ResBody;
							BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
							Log.Error($"Binance API Error: {error}");
						}
					}

					if (!string.IsNullOrEmpty(emir.BinanceTrailingOrderId))
					{
						var trailingDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BinanceTrailingOrderId);
						await _dbHelper.ProcessLogYazAsyncADO(emir.Id, trailingDetail.ReqPath, trailingDetail.ReqBody, trailingDetail.ResBody, "trailingDetail");

						if (trailingDetail.IsSuccess)
						{
							var status = trailingDetail.Result.status;
							if (status.Value == "FILLED")
							{
								if (emir.BinanceCloseOrderId != null)
								{
									await BinanceHelper.OrderSil(emir.Symbol.Replace(".P", ""), emir.BinanceCloseOrderId);
								}

								emir.Status = "CLOSED";
								emir.ClosedPrice = trailingDetail.Result.stopPrice;
								emir.ClosedDatetime = DateTime.Now;

								await _dbHelper.CoinPozisyonDurumUpdate(emir);
								Log.Information("Emir Durum CLOSED WITH TRAILING : " + emir.Symbol.ToString());
							}

							if (status.Value == "EXPIRED")
							{
								if (emir.BinanceCloseOrderId != null)
								{
									await BinanceHelper.OrderSil(emir.Symbol.Replace(".P", ""), emir.BinanceCloseOrderId);
								}

								emir.Status = "EXPIRED";
								emir.ClosePrice = trailingDetail.Result.stopPrice;
								emir.ClosedDatetime = DateTime.Now;

								await _dbHelper.CoinPozisyonDurumUpdate(emir);
								//_dbContext.SaveChanges();

								Log.Information("Emir Durum EXPIRED :" + emir.Symbol.Replace(".P", "").ToString());
							}
						}
						else
						{
							string errorJson = trailingDetail.ResBody;
							BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
							Log.Error($"Binance API Error: {error}");
						}
					}

					//if (!string.IsNullOrEmpty(emir.BINANCE_TP_ORDER_ID))
					//{
					//	var tpDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BINANCE_TP_ORDER_ID);
					//	await _dbHelper.ProcessLogYazAsyncADO(emir.Id, tpDetail.ReqPath, tpDetail.ReqBody, tpDetail.ResBody, "tpDetail");

					//	if (tpDetail.IsSuccess)
					//	{
					//		var status = tpDetail.Result.status;
					//		if (status.Value == "FILLED")
					//		{
					//			emir.Status = "CLOSED_TP";
					//			emir.ClosedPrice = tpDetail.Result.price;
					//			emir.ClosedDatetime = DateTime.Now;

					//			await BinanceHelper.OrderSil(emir.Symbol, emir.BINANCE_SL_ORDER_ID);
					//			await _dbHelper.CoinPozisyonDurumUpdate(emir);
					//			Log.Information("Emir Durum CLOSED WITH TP : " + emir.Symbol.ToString());
					//		}

					//		if (status.Value == "EXPIRED")
					//		{
					//			emir.Status = "EXPIRED";
					//			emir.ClosePrice = tpDetail.Result.price;
					//			emir.ClosedDatetime = DateTime.Now;

					//			await BinanceHelper.OrderSil(emir.Symbol, emir.BINANCE_SL_ORDER_ID);
					//			await _dbHelper.CoinPozisyonDurumUpdate(emir);
					//			//_dbContext.SaveChanges();

					//			Log.Information("Emir Durum EXPIRED :" + emir.Symbol.ToString());
					//		}
					//	}
					//	else
					//	{
					//		string errorJson = tpDetail.ResBody;
					//		BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
					//		Log.Error($"Binance API Error: {error}");
					//	}
					//}

					//if (!string.IsNullOrEmpty(emir.BINANCE_SL_ORDER_ID))
					//{
					//	var slDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BINANCE_SL_ORDER_ID);
					//	await _dbHelper.ProcessLogYazAsyncADO(emir.Id, slDetail.ReqPath, slDetail.ReqBody, slDetail.ResBody, "slDetail");

					//	if (slDetail.IsSuccess)
					//	{
					//		var status = slDetail.Result.status;
					//		if (status.Value == "FILLED")
					//		{
					//			emir.Status = "CLOSED_SL";
					//			emir.ClosedPrice = slDetail.Result.price;
					//			emir.ClosedDatetime = DateTime.Now;

					//			await BinanceHelper.OrderSil(emir.Symbol, emir.BINANCE_TP_ORDER_ID);
					//			await _dbHelper.CoinPozisyonDurumUpdate(emir);
					//			Log.Information("Emir Durum CLOSED WITH SL : " + emir.Symbol.ToString());
					//		}

					//		if (status.Value == "EXPIRED")
					//		{
					//			emir.Status = "EXPIRED";
					//			emir.ClosePrice = slDetail.Result.price;
					//			emir.ClosedDatetime = DateTime.Now;

					//			await BinanceHelper.OrderSil(emir.Symbol, emir.BINANCE_TP_ORDER_ID);
					//			await _dbHelper.CoinPozisyonDurumUpdate(emir);
					//			//_dbContext.SaveChanges();

					//			Log.Information("Emir Durum EXPIRED :" + emir.Symbol.ToString());
					//		}
					//	}
					//	else
					//	{
					//		string errorJson = slDetail.ResBody;
					//		BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
					//		Log.Error($"Binance API Error: {error}");
					//	}
					//}


				}
				catch (Exception e)
                {
                    Log.Error("PozisyonKontrol Hata " + e.Message);
                }
                i++;
            }));


            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            Log.Debug("PozisyonKontrol metodu Çalışma Süresi: " + elapsedTime + " işlem sayısı:" + i.ToString());
            return i;
        }
    }
}
