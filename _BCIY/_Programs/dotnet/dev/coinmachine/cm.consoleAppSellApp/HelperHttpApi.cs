using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cm.consoleAppSellApp
{
    public class HelperHttpApi
    {
        private static string apiKey = "F0zsjsgh2QCr7JM5C00JK8PtX12t1TWlls9IBAvlX8d18X5SfVbNB8gcJhjw61FW";
        private static string secretKey = "RJDLLnJh0F53XcjrZLbOuhss99RCG1Pt0Yu6PPTrEZ8b9XhVVJpQyiaUD3c00zbn";
        //private static string endpoint = "https://fapi.binance.com";
        private static string endpoint = "https://testnet.binance.vision";
        // Kaldıraç seviyesi (10x)
        private static int _leverage = 5;

        public static async Task<dynamic> account()
        {
            string path = "/fapi/v2/account";
            string method = "GET";

            // Unix epoch zamanı hesapla
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // İmza oluştur
            string queryString = $"timestamp={timestamp}";
            string signature = Sign(queryString, secretKey);

            // İstek URL'sini oluştur
            string url = $"{endpoint}{path}?{queryString}&signature={signature}";

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek ayarları
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                // İstek gönder
                HttpResponseMessage response = await client.GetAsync(url);

                // Yanıtı oku
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
                //Console.WriteLine(responseData);
            }
        }

        public static async Task<dynamic> balance()
        {
            string path = "/fapi/v2/balance";
            string method = "GET";

            // Unix epoch zamanı hesapla
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // İmza oluştur
            string queryString = $"timestamp={timestamp}";
            string signature = Sign(queryString, secretKey);

            // İstek URL'sini oluştur
            string url = $"{endpoint}{path}?{queryString}&signature={signature}";

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek ayarları
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                // İstek gönder
                HttpResponseMessage response = await client.GetAsync(url);

                // Yanıtı oku
                string responseData = await response.Content.ReadAsStringAsync();

                return responseData;
                //Console.WriteLine(responseData);
            }
        }

        public static async Task<dynamic> order()
        {
            string path = "/fapi/v1/order";
            string method = "POST";

            // Unix epoch zamanı hesapla
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();


            // İmza oluştur
            string queryString = $"symbol=BTCUSDT&side=BUY&type=MARKET&quantity=0.001&timestamp={timestamp}&isolatedMargin=true&leverage={_leverage}"; // isolatedMargin parametresi eklenmiştir
            string signature = Sign(queryString, secretKey);

            // İstek URL'sini oluştur
            string url = $"{endpoint}{path}?{queryString}&signature={signature}";

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek ayarları
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                // İstek gövdesi (body)
                string requestBody = "{\"symbol\":\"BTCUSDT\",\"side\":\"BUY\",\"type\":\"MARKET\",\"quantity\":\"0.001\",\"isolatedMargin\":true}"; 

                // İstek içeriği
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // İstek gönder
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Yanıtı oku
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
        }

        public class OrderResponse
        {
            public string Symbol { get; set; }
            public string OrderId { get; set; }
            public string ClientOrderId { get; set; }
            public string TransactTime { get; set; }
        }


        /// <summary>
        /// BUY or SELL
        /// </summary>
        /// <param name="symbol">BUY/SELL</param>
        /// <param name="emirtipi"></param>
        /// <param name="quantity">0.001</param>
        /// <returns></returns>
        public static async Task<dynamic> order(string symbol, string emirtipi, string quantity)
        {
            // Emir oluşturma endpoint'i
            string path = "/fapi/v1/order";

            // Unix epoch zamanı hesapla
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Emir bilgilerini içeren model
            var order = new
            {
                symbol = symbol,
                side = emirtipi,
                type = "MARKET",
                quantity = quantity,
                timestamp = timestamp,
                isolatedMargin = true,
                leverage = _leverage
            };

            // İmza oluştur
            string signature = Sign(order, secretKey);

            // İstek URL'sini oluştur
            string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}";

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek ayarları
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                // Emir bilgisini JSON formatına dönüştür
                string requestBody = JsonConvert.SerializeObject(order);

                // İstek gövdesi (body)
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // İstek gönder
                // İstek gönder
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Yanıtı oku ve model içine at
                string responseData = await response.Content.ReadAsStringAsync();
                OrderResponse orderResponse = JsonConvert.DeserializeObject<OrderResponse>(responseData);

                // Yanıtı oku
                //string responseData = await response.Content.ReadAsStringAsync();
                return orderResponse;
            }
        }

        public static async Task<dynamic> pmAccountInfo()
        {
            string path = "/fapi/v1/pmAccountInfo";
            string method = "GET";

            // Unix epoch zamanı hesapla
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // İmza oluştur
            string queryString = $"timestamp={timestamp}";
            string signature = Sign(queryString, secretKey);

            // İstek URL'sini oluştur
            string url = $"{endpoint}{path}?{queryString}&signature={signature}";

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek ayarları
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                // İstek gönder
                HttpResponseMessage response = await client.GetAsync(url);

                // Yanıtı oku
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
                //Console.WriteLine(responseData);
            }
        }


        static string Sign(string queryString, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        static string Sign(object data, string secretKey)
        {
            string jsonString = JsonConvert.SerializeObject(data);
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(jsonString));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
