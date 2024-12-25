using Common;
using Common.Dtos;
using Common.Model;
using Data;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workerServiceCoinAlSat.BackgroundTasks
{
	public class Step3TersEmirAcWorkerService : BackgroundService, IDisposable
	{
		private readonly ILogger<Step3TersEmirAcWorkerService> _logger;
		private readonly IConfiguration _configuration;
		private readonly DbHelper _dbHelper;
		private TimeSpan _period = TimeSpan.FromSeconds(10);
		private CoinsContext _dbContext;

		public Step3TersEmirAcWorkerService(IConfiguration configuration, ILogger<Step3TersEmirAcWorkerService> logger, CoinsContext dbContext)
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
					await TersEmirAcTRAILING();
				}
				catch (Exception e)
				{
					Log.Error("Step3TersEmirAcWorkerService Global Error:" + e.Message);
				}
			}
		}

		private async Task<int> TersEmirAcTRAILING()
		{
			List<PROCESS_DTO> yeniEmirler = await DbHelper.GuncellenecekProcessListesi(); // renew veya opened ve binance_close_order_id olmayan
			int i = 0;
			foreach (var emir in yeniEmirler)
			{
				try
				{
					Log.Information("Ters Emir Acilacak Emir No : " + emir.Id + "-" + emir.Symbol);
					string emirtipi = emir.OrderSide!;
					string tersTipi = string.Empty;
					string sembol = emir.Symbol;

					if (emirtipi == "BUY")
					{
						tersTipi = "SELL";
					}
					else
					{
						tersTipi = "BUY";
					}
					//var kapatEmri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: yeniemirTipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), StopPrice : emir.StopPrice.Value.ToString().Replace(",", "."),  type: "STOP/TAKE_PROFIT", orderID: emir.BinanceCloseOrderId);
					var trailingEmir = await BinanceHelper.OrderTRAILING_STOP_MARKET(symbol: emir.Symbol.Replace(".P", ""), emirtipi: tersTipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), type: "TRAILING_STOP_MARKET", ActivatePrice: emir.ClosePrice.Value.ToString().Replace(",", "."), CallbackRate: emir.CallBackRate.Value.ToString().Replace(",","."), orderID: emir.BinanceTrailingOrderId);
					
					var kapatEmri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: tersTipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), StopPrice : emir.StopPrice.Value.ToString().Replace(",", "."),  type: "STOP", orderID: emir.BinanceCloseOrderId);

					_dbHelper.ProcessLogYaz(emir.Id, trailingEmir.ReqPath, trailingEmir.ReqBody, trailingEmir.ResBody, "TRAILING_STOP_MARKET");

					if (trailingEmir.IsSuccess)
					{
						emir.BinanceTrailingOrderId = trailingEmir.Result.orderId;
						//emir.Status = "OPENED";
						emir.UpdateDatetime = DateTime.Now;
						await DbHelper.CoinTRAILINGDurumUpdate(emir);
						Log.Information("TRAILING STOP emir acildi" + sembol);
					}
					else
					{
						//emir.Status = "OPENED";
						emir.UpdateDatetime = DateTime.Now;
						await DbHelper.CoinTRAILINGDurumUpdate(emir);
						string errorJson = trailingEmir.ResBody;
						BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
						Log.Error("TRAILING STOP Emir Acilirken Binance Hatasi : " + errorJson);
					}

					if (kapatEmri.IsSuccess)
					{
						emir.BinanceCloseOrderId = kapatEmri.Result.orderId;
						//emir.Status = "OPENED";
						emir.UpdateDatetime = DateTime.Now;
						await DbHelper.CoinDurumUpdate(emir);
						Log.Information("STOP emir acildi" + sembol);
					}
					else
					{
						//emir.Status = "OPENED";
						emir.UpdateDatetime = DateTime.Now;
						await DbHelper.CoinDurumUpdate(emir);
						string errorJson = kapatEmri.ResBody;
						BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
						Log.Error("STOP Emir Acilirken Binance Hatasi : " + errorJson);
					}
				}
				catch (Exception e)
				{
					_logger.LogError("TRAILING Emir Acilirken Hata" + e.Message);
				}
				i++;
			}
			return i;
		}

		//private async Task<int> TersEmirAc()
		//{
		//	//var yeniEmirler = _dbContext.Processes.Where(x => new[] { "RENEW" }.Contains(x.Status) && x.InsertDatetime.Value.AddMinutes(-1) >= DateTime.Now ).ToList();
		//	List<PROCESS_DTO> yeniEmirler = await DbHelper.GuncellenecekProcessListesi(); // renew veya opened ve binance_close_order_id olmayan
		//	int i = 0;
		//	foreach (var emir in yeniEmirler)
		//	{
		//		try
		//		{
		//			Log.Information("Ters Emir Acilacak Emir No : " + emir.Id + "-" + emir.Symbol);
		//			string emirtipi = emir.OrderSide!;
		//			string tersTipi = string.Empty;
		//			string sembol = emir.Symbol;

		//			if (emirtipi == "BUY")
		//			{
		//				tersTipi = "SELL";
		//			}
		//			else
		//			{
		//				tersTipi = "BUY";
		//			}

		//			//var kapatEmri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: yeniemirTipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), StopPrice : emir.StopPrice.Value.ToString().Replace(",", "."),  type: "STOP/TAKE_PROFIT", orderID: emir.BinanceCloseOrderId);
		//			//var kapatEmri = await BinanceHelper.OrderTRAILING_STOP_MARKET(symbol: emir.Symbol.Replace(".P", ""), emirtipi: yeniemirTipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), type: "TRAILING_STOP_MARKET", ActivatePrice: emir.ClosePrice.Value.ToString().Replace(",", "."), CallbackRate: emir.CallBackRate.Value.ToString().Replace(",","."), orderID: emir.BinanceCloseOrderId);

		//			//var tpEmri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: emirtipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), StopPrice: emir.ClosePrice.Value.ToString().Replace(",", "."), type: "TAKE_PROFIT", "");

		//			// TAKE PROFIT EMRİ
		//			var tpEmri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: tersTipi, quantity: emir.ExecutedQuantity.ToString().Replace(",", "."), amount: emir.ClosePrice.Value.ToString().Replace(",", "."), StopPrice: emir.ClosePrice.Value.ToString().Replace(",", "."), type: "TAKE_PROFIT_MARKET", emir.BINANCE_TP_ORDER_ID);
		//			_dbHelper.ProcessLogYaz(emir.Id, tpEmri.ReqPath, tpEmri.ReqBody, tpEmri.ResBody, "TP_EMIR_AC");
		//			//emir.BINANCE_TP_ORDER_ID = emir.Id.ToString();

		//			if (tpEmri.IsSuccess)
		//			{
		//				emir.BinanceCloseOrderId = tpEmri.Result.orderId;
		//				emir.Status = "OPENED";
		//				emir.UpdateDatetime = DateTime.Now;
		//				await DbHelper.CoinTPDurumUpdate(emir);
		//				Log.Information("TP emir acildi" + sembol);
		//			}
		//			else
		//			{
		//				emir.Status = "OPENED";
		//				emir.UpdateDatetime = DateTime.Now;
		//				await DbHelper.CoinSLDurumUpdate(emir);
		//				string errorJson = tpEmri.ResBody;
		//				BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
		//				Log.Error("TP Emir Acilirken Binance Hatasi : " + errorJson);
		//			}


		//			// STOP SLOSS İÇİN STOP_MARKET EMRİ
		//			var slEmri = await BinanceHelper.Order(symbol: emir.Symbol.Replace(".P", ""), emirtipi: tersTipi, quantity: emir.ExecutedQuantity.ToString(), amount: emir.StopPrice.Value.ToString().Replace(",", "."), StopPrice: emir.StopPrice.Value.ToString().Replace(",", "."), type: "STOP_MARKET", emir.BINANCE_SL_ORDER_ID);
		//			_dbHelper.ProcessLogYaz(emir.Id, slEmri.ReqPath, slEmri.ReqBody, slEmri.ResBody, "SL_EMIR_AC");
		//			if (slEmri.IsSuccess)
		//			{
		//				emir.BinanceCloseOrderId = slEmri.Result.orderId;
		//				emir.Status = "OPENED";
		//				emir.UpdateDatetime = DateTime.Now;
		//				await DbHelper.CoinSLDurumUpdate(emir);
		//				Log.Information("SL emir acildi" + sembol);
		//			}
		//			else
		//			{
		//				emir.Status = "OPENED";
		//				emir.UpdateDatetime = DateTime.Now;
		//				await DbHelper.CoinTPDurumUpdate(emir);
		//				string errorJson = slEmri.ResBody;
		//				BinanceApiErrorResponse error = JsonConvert.DeserializeObject<BinanceApiErrorResponse>(errorJson);
		//				Log.Error("SL Emir Acilirken Binance Hatasi : " + errorJson);
		//			}
		//		}
		//		catch (Exception e)
		//		{
		//			_logger.LogError("Ters Emir Acilirken Hata" + e.Message);
		//		}
		//		i++;
		//	}
		//	return i;
		//}
	}
}
