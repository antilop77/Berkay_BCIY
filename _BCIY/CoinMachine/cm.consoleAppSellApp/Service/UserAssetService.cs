using Binance.Common;
using Binance.Spot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cm.consoleAppSellApp.Service
{
    public class UserAssetService 
    {
        private string apiKey = "F0zsjsgh2QCr7JM5C00JK8PtX12t1TWlls9IBAvlX8d18X5SfVbNB8gcJhjw61FW";
        private string apiSecret = "RJDLLnJh0F53XcjrZLbOuhss99RCG1Pt0Yu6PPTrEZ8b9XhVVJpQyiaUD3c00zbn";



        public async Task UserAssetGet()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<UserAsset>();

            HttpMessageHandler loggingHandler = new BinanceLoggingHandler(logger: logger);
            HttpClient httpClient = new HttpClient(handler: loggingHandler);

            //string apiKey = "api-key";
            //string apiSecret = "api-secret";

            var wallet = new Wallet(httpClient, apiKey: apiKey, apiSecret: apiSecret);

            var result = await wallet.UserAsset();
        }

    }
}
