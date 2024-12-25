using Binance.Net.Clients;
using Common;
using CryptoExchange.Net.Authentication;
using Dapper;
using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using workerServiceCoinAlSat.Services;

namespace workerServiceCoinAlSat.BackgroundTasks
{
    public class WorkerTest : BackgroundService
    {
        private readonly ILogger<WorkerTest> _logger;
        private readonly IConfiguration _configuration;
        private readonly DbHelper _dbHelper;
        private readonly BinanceHelper _binanceHelper;
		private CoinsContext _dbContext;
		public WorkerTest(IConfiguration configuration, ILogger<WorkerTest> logger, CoinsContext dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dbHelper = new DbHelper();
            _binanceHelper = new BinanceHelper();
			_dbContext = dbContext;
		}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
				test();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(600000, stoppingToken);
            }
        }

        public void sqlhiztesti()
        {
            string connectionString = _configuration.GetConnectionString("sql_coin");
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"SELECT IndikatorAdi, IndikatorDegeri, GelenVeriZamani, IslemZamanBirimi " +
                                "FROM IndikatorVerileri with (NOLOCK) " +
                                "WHERE CoinKodu = @coinkodu AND IndikatorAdi IN ('high', 'low', 'VWMA', 'close') " +
                                "AND IslemZamanBirimi IN ('1m', '15m') " +
                                "AND GelenVeriZamani >= CONVERT(datetime, @prmtarih, 120) " +
                                "ORDER BY GelenVeriZamani";

                    var parameters = new DynamicParameters();
                    parameters.Add("@coinkodu", "AVAXUSDT.P");
                    parameters.Add("@prmtarih", new DateTime(2024, 4, 28, 02, 30, 0));

                    var result = connection.Query<dynamic>(sql, parameters);

                    foreach (var item in result)
                    {
                        var aa = item.CoinKodu;
                        //Console.WriteLine($"Id: {item.CoinKodu}");
                    }
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
                throw;
            }
        }

		public async Task test2()
        {
			//var binanceClient = new BinanceRestClient(options =>
			//{
			//	options.ApiCredentials = new ApiCredentials(apiKey, secretKey);
			//});

			//// Tüm tick verilerini al
			//var tickers = await binanceClient.FuturesUsdt.Market.Get24HPricesAsync();

			//if (tickers.Success)
			//{
			//	var tickerData = tickers.Data;

			//	// Yüzde değişim hesapla ve listele
			//	var changeData = tickerData.Select(t => new
			//	{
			//		Symbol = t.Symbol,
			//		PercentageChange = ((t.LastPrice - t.OpenPrice) / t.OpenPrice) * 100
			//	}).ToList();

			//	// En çok artan coinler
			//	var topGainers = changeData.OrderByDescending(t => t.PercentageChange).Take(10);

			//	// En çok azalan coinler
			//	var topLosers = changeData.OrderBy(t => t.PercentageChange).Take(10);

			//	Console.WriteLine("En çok artan coinler:");
			//	foreach (var gainer in topGainers)
			//	{
			//		Console.WriteLine($"Symbol: {gainer.Symbol}, Change: {gainer.PercentageChange:F2}%");
			//	}

			//	Console.WriteLine("\nEn çok azalan coinler:");
			//	foreach (var loser in topLosers)
			//	{
			//		Console.WriteLine($"Symbol: {loser.Symbol}, Change: {loser.PercentageChange:F2}%");
			//	}
			//}
			//else
			//{
			//	Console.WriteLine("Tick verileri alınamadı.");
			//}
		}


		public async Task test()
        {
			string SEMBOL = "ETHUSDT";

		
			//var acEmri = await BinanceHelper.Order(symbol: "ETHUSDT", emirtipi: "BUY", quantity: "1", amount: "3717.04", StopPrice: "3700.04", type: "MARKET", "");
			var kapatEmri = await BinanceHelper.Order(symbol: SEMBOL, emirtipi: "SELL", quantity: "1", amount: "3600.04", StopPrice: "3600.04", type: "STOP_MARKET", orderID: null);

			//string order1 = "7705593142";
			//string SEMBOL = "IOTAUSDT";
   //         var tumOrderKontrol = _dbContext.Processes.ToList();

   //         foreach (var emir in tumOrderKontrol)
   //         {
   //             string trOrderId = emir.BinanceTrailingOrderId;
   //             string closeOrderId = emir.BinanceCloseOrderId;
			//	var trailingDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BinanceTrailingOrderId);
			//	var status = trailingDetail.Result.status;
			//	if (status.Value == "FILLED")
			//	{
			//		await BinanceHelper.OrderSil(emir.Symbol.Replace(".P", ""), closeOrderId);
			//	}

			//	var closeDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BinanceCloseOrderId);
			//	var closestatus = trailingDetail.Result.status;
			//	if (closestatus == "FILLED")
			//	{
			//		await BinanceHelper.OrderSil(emir.Symbol.Replace(".P", ""), trOrderId);
			//	}
			//}
			//Console.WriteLine("bitti");
			////var orderDetail = await BinanceHelper.GetOrderDetail(emir.Symbol.Replace(".P", ""), emir.BinanceCloseOrderId);

			//string openPrice = "3969.04";
   //         string closePrice = "4000.12";
   //         string stopPrice = "3800";
   //         string tpPrice = "4100";
		////	var acEmri = await BinanceHelper.OrderOCO(symbol: "ETHUSDT", emirtipi: "BUY", quantity: "1", Price: openPrice, StopPrice: closePrice, "");
	//var acEmri = await BinanceHelper.Order(symbol: "ETHUSDT", emirtipi: "BUY", quantity: "1", amount: openPrice, StopPrice: closePrice, type: "MARKET", "");
	//		var tpEmri = await BinanceHelper.Order(symbol: "ETHUSDT", emirtipi: "SELL", quantity: "1", amount: closePrice, StopPrice: stopPrice, type: "TAKE_PROFIT", "");
		
          //  var stopEmri = await BinanceHelper.Order(symbol: "ETHUSDT", emirtipi: "SELL", quantity: "1", amount: closePrice, StopPrice: stopPrice, type: "STOP_MARKET", "");
            //_dbHelper.ProcessLogYaz(0, acEmri.ReqPath, acEmri.ReqBody, acEmri.ResBody, "TEST_EMIR_AC");
   //         string bir = "";
			//var tpsEmri = await BinanceHelper.Order(symbol: "ETHUSDT", emirtipi: "SELL", quantity: "1", amount: closePrice, StopPrice: tpPrice, type: "TAKE_PROFIT_MARKET", "");

   //         await BinanceHelper.OrderSil("ETHUSDT", "1381636362");
            //var al = await BinanceHelper.Order(symbol: "ETHUSDT", emirtipi: "BUY", quantity: "1", amount: "2999.03", StopPrice :"3000", type: "MARKET");
            string test2 = "";
			//var orderDetail = await BinanceHelper.OrderTRAILING_STOP_MARKET("ETHUSDT", emirtipi:"BUY", "1", amount: "3000", "TRAILING_STOP_MARKET", "");
			//var orderDetail = await CoinHelper.GETODERDERDETAIL("BANDUSDT", "12344018395");

			//var al = await CoinHelper.order(symbol: "ETHUSDT", emirtipi: "BUY", quantity: "1", amount: "3601.00", type: "LIMIT");
			//BinanceHelper.ProcessLogYaz(1, "", "", "");
			//   _dbHelper.GetAllActiveCoinList();
			//   sqlhiztesti();
			//   int mum = await CoinHelper.getbinanceMum();
			// ORDER DEĞİŞİKLİĞİ
			var A = AppSettings.endPoint;
            // ALMA EMRİMİZ
     //       var al = await CoinHelper.order(symbol: "ETHUSDT", emirtipi: "SELL", quantity: "2", amount: "3601.00", type: "LIMIT");
            // GÜNCELLEME EMRİMİZ :
      //      var guncelle = await CoinHelper.order(symbol: "ETHUSDT", emirtipi: "SELL", quantity: "6", amount: "3315.00", type: "LIMIT", orderID: "1368901762");


            var test = "";

            //var degistir = await CoinHelper.orderModify("ETHUSDT", "4", 1);

            //      var orderDetail = await CoinHelper.GETODERDERDETAIL("ETHUSDT", "1365113138");
            //  var coin = await _dbHelper.GetCoinDetay("ETHUSDT.P");

            //var positionRisk = await BinanceHelper.positionRisk("ETHUSDT");


            //  var al = await CoinHelper.order(symbol:  "ETHUSDT", emirtipi: "BUY", quantity: "4", amount : "3264.00", type : "LIMIT");

            //   string orderbilgi = al.OrderId;
            // BU kapanma değeri istiyor!!
            //var kapat = await CoinHelper.orderCLOSE_POSITION("ETHUSDT", "BUY", "0.078", "", "STOP_MARKET");

            // var degistir = await CoinHelper.orderModify("ETHUSDT", "3197.55", 1);

            // var positionRisk = await CoinHelper.positionRisk("ETHUSDT");


            // var kapat = await CoinHelper.orderCLOSE_POSITION_MARKET("ETHUSDT", "SELL", "0.078", "MARKET", "");
            //var balance = await CoinHelper.balance();

            //var coin =  await _dbHelper.GetCoinDetay("NEOUSDT.P");

            //var aldenendi = CoinHelper.order("ETHUSDT", "BUY", "0.007", "3174.12");

            //var al = await CoinHelper.order("ETHUSDT", "BUY", "0.050", "3197.00");
            // var alOrderID = al.OrderId;
            //  string orderID = alOrderID;

           
            string order2 = "1363960398";
            //  var SAT = await CoinHelper.order("ETHUSDT", "BUY", "0.010", "3202.99", "LIMIT", order1);

            // 1 pozisyon ekle 2 pozisyon çıkar
            //  var degistir = await CoinHelper.orderModify("ETHUSDT", "3201.00", 1);

            //var degistir = CoinHelper.orderModify("ETHUSDT", "BUY", "0.007", "3900.11");

            //var sat = CoinHelper.order("NEOUSDT", "SELL", "0.001");

            Console.WriteLine("TEST");
            //List<CoinListesi> coinListesi = _dbHelper.GetAllCoinListesi();

            //foreach (CoinListesi coin in coinListesi)
            //{
            //    Console.WriteLine($"Id: {coin.CoinKodu}");

            //}
            //string connectionString = _configuration.GetConnectionString("sql_coin");
            //try
            //{

            //    // Dapper'� kullanarak sorgu �a�r�s�
            //    using (var connection = new SqlConnection(connectionString))
            //    {
            //        connection.Open();

            //        // SQL sorgusu
            //        string sql = "SELECT * FROM CoinListesi";

            //        // Sorguyu �al��t�r ve sonucu liste olarak al
            //        var result = connection.Query<dynamic>(sql);

            //        // D�nen sonucu kullanabilirsiniz
            //        foreach (var item in result)
            //        {
            //            Console.WriteLine($"Id: {item.CoinKodu}");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    string error = e.Message;
            //    throw;
            //}

            //await Task.Delay(5000);
            Task.Delay(5000).Wait();
            Console.WriteLine("test");

        }
    }
}