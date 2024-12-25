using Binance.Common;
using Binance.Spot;
using Binance.Spot.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Binance.Spot.Models;


namespace cm.consoleAppSellApp
{
    public class Helper
    {
        private static string apiKey = "F0zsjsgh2QCr7JM5C00JK8PtX12t1TWlls9IBAvlX8d18X5SfVbNB8gcJhjw61FW";
        private static string apiSecret = "RJDLLnJh0F53XcjrZLbOuhss99RCG1Pt0Yu6PPTrEZ8b9XhVVJpQyiaUD3c00zbn";

        public static async Task<string> UserAssetGet()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Helper>();

            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);

            //string apiKey = "api-key";
            //string apiSecret = "api-secret";

            var wallet = new Wallet(httpClient, apiKey: apiKey, apiSecret: apiSecret);

            var result = await wallet.UserAsset();

            return result;
        }

       

        public static async Task<string> CoinDigitSayisiLimit()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Helper>();

            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);


            var portfolioMargin = new PortfolioMargin(httpClient, apiKey: apiKey, apiSecret: apiSecret);

            var result = await portfolioMargin.PortfolioMarginCollateralRate();

            return result;
        }

        public static async Task<string> marginAl()
        {
            //// Binance Futures API istemcisini oluşturun
            //var client = new BinanceFuturesClient(apiKey, secretKey);

            //// Yeni bir BTCUSDT Market emri oluşturun
            //var order = new CreateOrder()
            //{
            //    Symbol = "BTCUSDT",
            //    Side = OrderSide.Buy, // LONG pozisyon için "Buy"
            //    Type = OrderType.Market, // Piyasa fiyatıyla emir
            //    Quantity = 0.01 // Sözleşme adedi
            //};

            //// Emri gönderin
            //var result = client.Order.CreateOrder(order);

            // Sipariş ID'sini ve işlemin durumunu yazdırın
            //Console.WriteLine("Sipariş ID: {0}", result.OrderId);
            //Console.WriteLine("İşlem Durumu: {1}", result.OrderStatus);

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Helper>();

            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);

            var marginAccountTrade = new MarginAccountTrade(httpClient, apiKey: apiKey, apiSecret: apiSecret);
            var marginassets = marginAccountTrade.GetAllMarginAssets();
            //var result = await marginAccountTrade.MarginAccountNewOrder("BNBUSDT", Side.SELL, OrderType.LIMIT, true, miktar_kacadet, );
            return "0";
        }

        public static async Task<string> wsTest()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Helper>();

           // string apiSecret = "apiSecret";
            //var websocket = new WebSocketApi("wss://testnet.binance.vision/ws-api/v3", apiKey, new BinanceHmac(apiSecret));
            var websocket = new WebSocketApi("wss://ws-api.binance.com:443/ws-api/v3", apiKey, new BinanceHmac(apiSecret));
            ;
            websocket.OnMessageReceived(
                async (data) =>
                {
                    logger.LogInformation(data);
                    await Task.CompletedTask;
                }, CancellationToken.None);

            await websocket.ConnectAsync(CancellationToken.None);

            await websocket.AccountTrade.AccountOrderRateLimitsAsync();
            await websocket.AccountTrade.AccountOrderRateLimitsAsync(recvWindow: 6000, requestId: 123);
            await websocket.AccountTrade.AccountOrderRateLimitsAsync(recvWindow: 6000, cancellationToken: CancellationToken.None);

            await Task.Delay(3000);
            logger.LogInformation("Disconnect with WebSocket Server");
            await websocket.DisconnectAsync(CancellationToken.None);

            return "1";
        }
    }
}
