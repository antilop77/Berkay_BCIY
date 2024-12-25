using Common;
using Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace workerServiceCoinAlSat.Services
{
    public class CoinHelper
    {
        // live cengiz
        //private static string apiKey = "F0zsjsgh2QCr7JM5C00JK8PtX12t1TWlls9IBAvlX8d18X5SfVbNB8gcJhjw61FW";
        //private static string secretKey = "RJDLLnJh0F53XcjrZLbOuhss99RCG1Pt0Yu6PPTrEZ8b9XhVVJpQyiaUD3c00zbn";
        //private static string endpoint = "https://fapi.binance.com";
        private static string endpoint = AppSettings.endPoint; // "https://testnet.binancefuture.com";

        /*
         test API Key
6f741d8722979ad85cdbb7c4d94c7a0de05a73d9507fad50a0921984567ac3dc
API Secret
bd08ec0fe4242762f02e0f1843d6a98102cc37ddd336846d95fc7e69c81a5599
         */
        // Kaldıraç seviyesi (10x)
        private static string apiKey = AppSettings.apiKey; // "6f741d8722979ad85cdbb7c4d94c7a0de05a73d9507fad50a0921984567ac3dc";
        private static string secretKey = AppSettings.secretKey; // "bd08ec0fe4242762f02e0f1843d6a98102cc37ddd336846d95fc7e69c81a5599";

        private static int _leverage = 5;

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

        public static async Task<dynamic> account()
        {
            string path = "/fapi/v2/account";
            string method = "GET";

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

        /// <summary>
        /// BUY or SELL
        /// </summary>
        /// <param name="symbol">BUY/SELL</param>
        /// <param name="emirtipi"></param>
        /// <param name="quantity">0.001</param>
        /// <returns></returns>
        public static async Task<dynamic> order(string symbol, string emirtipi, string quantity, string amount, string type = "LIMIT", string orderID = "")
        {
            try
            {
                // Emir oluşturma endpoint'i
                string path = "/fapi/v1/order";
                //string type = "LIMIT";
                // Unix epoch zamanı hesapla
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Emir bilgilerini içeren model
                var order = new
                {
                    orderId = orderID,
                    symbol = symbol,
                    side = emirtipi,
                    type = type,
                    quantity = quantity,
                    price = amount,
                    //timestamp = timestamp,
                    isolatedMargin = true,
                    leverage = _leverage,
                    timeInForce = "GTC",
                    marginType = "ISOLATED",
                    closePosition = true,
                    priceProtect = false
                };

                // İmza oluştur
                string queryString = "";
                queryString = $"symbol={order.symbol}&side={order.side}&type={order.type}&priceProtect={order.priceProtect}&orderId={order.orderId}&closePosition={order.closePosition}&quantity={order.quantity}&timestamp={timestamp}";

            //    if (order.type != "MARKET")
                {
                    queryString = queryString + $"&timeInForce={order.timeInForce}";
                    queryString = queryString + $"&price={order.price}";
                }

                //if (!string.IsNullOrEmpty(order.orderId))
                //{
                //    queryString = $"symbol={order.symbol}&side={order.side}&type={order.type}&orderId={order.orderId}&closePosition={order.closePosition}&quantity={order.quantity}&timestamp={timestamp}";

                //    if (order.type != "MARKET")
                //    {
                //        queryString = queryString + $"&timeInForce={order.timeInForce}";
                //        queryString = queryString + $"&price={order.price}";
                //    }
                //}
                //else
                //{
                //    queryString = $"symbol={order.symbol}&side={order.side}&type={order.type}&timeInForce={order.timeInForce}&quantity={order.quantity}&price={order.price}&timestamp={timestamp}";
                //}

                // İmza oluştur
                string signature = Sign(queryString, secretKey);

                // İstek URL'sini oluştur
                string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}&symbol={updateOrder.symbol}&orderId={updateOrder.orderId}&timeInForce={updateOrder.timeInForce}&quantity={updateOrder.quantity}";
                //
                // string queryString = $"timestamp={timestamp}&amount={amount}&symbol={symbol}&side={emirtipi}&quantity={quantity}&type={type}&timeInForce={{updateOrder.timeInForce}}";
                // string signature = Sign(queryString, secretKey);

                //string signature = Sign(order, secretKey);

                // İstek URL'sini oluştur
                //string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}";
                //string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // HTTP isteği oluştur
                using (HttpClient client = new HttpClient())
                {
                    // İstek ayarları
                    client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                    // Emir bilgisini JSON formatına dönüştür
                    //string requestBody = JsonConvert.SerializeObject(order);
                    string requestBody = string.Empty;

                    // İstek gövdesi (body)
                    HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    // İstek gönder
                    HttpResponseMessage response;
                    if (!string.IsNullOrEmpty(order.orderId))
                    {
                        response = await client.PutAsync(url, content);
                    }
                    else
                    {
                        response = await client.PostAsync(url, content);
                    }
                    // Yanıtı oku ve model içine at
                    string responseData = await response.Content.ReadAsStringAsync();
                    OrderResponse orderResponse = JsonConvert.DeserializeObject<OrderResponse>(responseData);

                    // Yanıtı oku
                    //string responseData = await response.Content.ReadAsStringAsync();
                    return orderResponse;
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                return null;
            }
        }


        /// <summary>
        /// Mandatory parameter 'stopprice' was not sent, was empty/null, or malformed
        /// STOP_MARKET verdiğimizde stopprice vermemizi istiyor!!
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="emirtipi"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static async Task<dynamic> orderCLOSE_POSITION(string symbol, string emirtipi, string quantity, string amount, string type = "", string orderID = "")
        {
            try
            {
                // Emir oluşturma endpoint'i
                string path = "/fapi/v1/order";
                //string type = "LIMIT";
                // Unix epoch zamanı hesapla
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Emir bilgilerini içeren model
                var order = new
                {
                    orderId = orderID,
                    symbol = symbol,
                    side = emirtipi,
                    type = type,
                    price = amount,
                    //timestamp = timestamp,
                    isolatedMargin = true,
                    timeInForce = "GTC",
                    closePosition = true,
                    stopPrice = "STOP"
                };

                // İmza oluştur
                string queryString = "";
                queryString = $"symbol={order.symbol}&side={order.side}&type={order.type}&stopPrice={order.stopPrice}&closePosition={order.closePosition}&timestamp={timestamp}";


                // İmza oluştur
                string signature = Sign(queryString, secretKey);

                // İstek URL'sini oluştur
                string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}&symbol={updateOrder.symbol}&orderId={updateOrder.orderId}&timeInForce={updateOrder.timeInForce}&quantity={updateOrder.quantity}";
                //
                // string queryString = $"timestamp={timestamp}&amount={amount}&symbol={symbol}&side={emirtipi}&quantity={quantity}&type={type}&timeInForce={{updateOrder.timeInForce}}";
                // string signature = Sign(queryString, secretKey);

                //string signature = Sign(order, secretKey);

                // İstek URL'sini oluştur
                //string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}";
                //string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // HTTP isteği oluştur
                using (HttpClient client = new HttpClient())
                {
                    // İstek ayarları
                    client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                    // Emir bilgisini JSON formatına dönüştür
                    //string requestBody = JsonConvert.SerializeObject(order);
                    string requestBody = string.Empty;

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
            catch (Exception e)
            {
                string message = e.Message;
                return null;
            }
        }


        public static async Task<dynamic> orderCLOSE_POSITION_MARKET(string symbol, string emirtipi, string quantity, string type = "", string orderID = "")
        {
            try
            {
                // Emir oluşturma endpoint'i
                string path = "/fapi/v1/order";
                //string type = "LIMIT";
                // Unix epoch zamanı hesapla
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Emir bilgilerini içeren model
                var order = new
                {
                    orderId = orderID,
                    symbol = symbol,
                    side = emirtipi,
                    type = type,
                    //timestamp = timestamp,
                    timeInForce = "GTC",
                    closePosition = false,
                    stopPrice = "STOP",
                    quantity = quantity
                };

                // İmza oluştur
                string queryString = "";
                queryString = $"symbol={order.symbol}&side={order.side}&type={order.type}&quantity={order.quantity}&closePosition={order.closePosition}&timestamp={timestamp}";


                // İmza oluştur
                string signature = Sign(queryString, secretKey);

                // İstek URL'sini oluştur
                string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // HTTP isteği oluştur
                using (HttpClient client = new HttpClient())
                {
                    // İstek ayarları
                    client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                    // Emir bilgisini JSON formatına dönüştür
                    //string requestBody = JsonConvert.SerializeObject(order);
                    string requestBody = string.Empty;

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
            catch (Exception e)
            {
                string message = e.Message;
                return null;
            }
        }

        public static async Task<dynamic> orderKAPAT(string orderid, string symbol, string emirtipi, string quantity, string amount)
        {
            // Emir oluşturma endpoint'i
            string path = "/fapi/v1/order";
            string type = "LIMIT";
            // Unix epoch zamanı hesapla
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Emir bilgilerini içeren model
            //var order = new
            //{
            //    symbol = symbol,
            //    side = emirtipi,
            //    type = "MARKET",
            //    quantity = quantity,
            //    //timestamp = timestamp,
            //    isolatedMargin = true,
            //    leverage = _leverage
            //};

            // İmza oluştur
            string queryString = $"timestamp={timestamp}&amount={amount}&symbol={symbol}&orderId={orderid}&side={emirtipi}&quantity={quantity}&type={type}";
            string signature = Sign(queryString, secretKey);

            //string signature = Sign(order, secretKey);

            // İstek URL'sini oluştur
            //string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}";
            string url = $"{endpoint}{path}?{queryString}&signature={signature}";

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek ayarları
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                // Emir bilgisini JSON formatına dönüştür
                //string requestBody = JsonConvert.SerializeObject(order);
                string requestBody = string.Empty;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="amount"></param>
        /// <param name="type">1: Add position margin，2: Reduce position margin</param>
        /// <returns></returns>
        public static async Task<dynamic> orderModify(string symbol, string amount, int type = 1)
        {
            try
            {
                // Emir oluşturma endpoint'i
                string path = "/fapi/v1/positionMargin";
                //string type = "LIMIT";

                // Unix epoch zamanı hesapla
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Emir bilgilerini içeren model
                //var order = new
                //{
                //    symbol = symbol,
                //    side = emirtipi,
                //    type = "MARKET",
                //    quantity = quantity,
                //    timestamp = timestamp,
                //    isolatedMargin = true,
                //    leverage = _leverage
                //};

                // İmza oluştur
                //string signature = Sign(order, secretKey);

                // İstek URL'sini oluştur
                //string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}&amount={amount}&symbol={symbol}&side={emirtipi}&quantity={quantity}&type={type}";
                string queryString = $"timestamp={timestamp}&amount={amount}&type={type}&symbol={symbol}";
                string signature = Sign(queryString, secretKey);
                string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // HTTP isteği oluştur
                using (HttpClient client = new HttpClient())
                {
                    // İstek ayarları
                    client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                    // Emir bilgisini JSON formatına dönüştür
                    string requestBody = string.Empty;

                    // İstek gövdesi (body)
                    HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

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
            catch (Exception e)
            {
                string _hata = e.Message;
                throw;
                return null;

            }
        }

        public class binancepositionRiskReq
        {
            public string timestamp { get; set; }
            public string symbol { get; set; }
        }

        public static async Task<dynamic> positionRisk(string symbol)
        {
            try
            {
                // Emir oluşturma endpoint'i
                string path = "/fapi/v2/positionRisk";
                //string type = "LIMIT";

                // Unix epoch zamanı hesapla
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                var queryModel = new
                {
                    timestamp = timestamp,
                    symbol = symbol,

                };

                // İstek URL'sini oluştur
                //string url = $"{endpoint}{path}?timestamp={timestamp}&signature={signature}&amount={amount}&symbol={symbol}&side={emirtipi}&quantity={quantity}&type={type}";
                string queryString = $"timestamp={timestamp}&symbol={queryModel.symbol}";
                string signature = Sign(queryString, secretKey);
                string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                // HTTP isteği oluştur
                using (HttpClient client = new HttpClient())
                {
                    // İstek ayarları
                    client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                    // Emir bilgisini JSON formatına dönüştür
                    string requestBody = string.Empty;

                    // İstek gövdesi (body)
                    HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    // İstek gönder
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Yanıtı oku ve model içine at
                    string responseData = await response.Content.ReadAsStringAsync();
                    OrderResponse orderResponse = JsonConvert.DeserializeObject<OrderResponse>(responseData);

                    // Yanıtı oku
                    //string responseData = await response.Content.ReadAsStringAsync();
                    return orderResponse;
                }
            }
            catch (Exception e)
            {
                string _hata = e.Message;
                throw;
                return null;

            }
        }



        public static async Task<dynamic> GETODERDERDETAIL(string symbol, string orderID)
        {
            try
            {
                string path = "/fapi/v1/order";
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                var order = new
                {
                    orderId = orderID,
                    symbol = symbol
                };

                string queryString = "";
                queryString = $"symbol={order.symbol}&orderId={order.orderId}&timestamp={timestamp}";

                string signature = Sign(queryString, secretKey);

                string url = $"{endpoint}{path}?{queryString}&signature={signature}";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

                    string requestBody = string.Empty;

                    // İstek gövdesi (body)
                    HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseData = await response.Content.ReadAsStringAsync();
                    OrderResponse orderResponse = JsonConvert.DeserializeObject<OrderResponse>(responseData);

                    return orderResponse;
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                return null;
            }
        }

        static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix zamanı 1 Ocak 1970'ten bu yana geçen saniye sayısına dönüştür
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // Unix zaman damgasını kullanarak DateTime nesnesini oluştur
            return unixEpoch.AddMilliseconds(unixTimeStamp).ToLocalTime();
        }

        public static async Task<int> getbinanceMum()
        {
            string symbol = "BTCUSDT"; // İlgilenilen sembol

            // Binance API endpoint'i
            string endpoint = "https://api.binance.com";

            // 15 dakikalık mum verileri endpoint'i
            string path = $"/api/v3/klines?symbol={symbol}&interval=15m&limit=2"; // Son 1 mumu al

            // HTTP isteği oluştur
            using (HttpClient client = new HttpClient())
            {
                // İstek gönder
                HttpResponseMessage response = await client.GetAsync(endpoint + path);

                // Yanıtı oku
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var klines = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<object>>>(responseData);

                    if (klines.Count > 0)
                    {
                        // Kapanış (close), açılış (open), en yüksek (high) ve en düşük (low) fiyatları al
                        decimal open = Convert.ToDecimal(klines[0][1]);
                        decimal high = Convert.ToDecimal(klines[0][2]);
                        decimal low = Convert.ToDecimal(klines[0][3]);
                        decimal close = Convert.ToDecimal(klines[0][4]);
                        decimal volume = Convert.ToDecimal(klines[0][5]);
                        long _datetime = Convert.ToInt64(klines[0][6]);

                        DateTime tarih = UnixTimeStampToDateTime(_datetime);
                        Console.WriteLine($"Açılış: {open}, Yüksek: {high}, Düşük: {low}, Kapanış: {close}");
                    }
                    else
                    {
                        Console.WriteLine("Veri bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine($"HTTP hatası: {response.StatusCode}");
                }
            }

            return 1;
        }

    }
}
