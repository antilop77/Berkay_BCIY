using Binance.Common;
using Binance.Spot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using cm.consoleAppSellApp.Service;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;

namespace cm.consoleAppSellApp
{
    internal class Program
    {

    

        /// F0zsjsgh2QCr7JM5C00JK8PtX12t1TWlls9IBAvlX8d18X5SfVbNB8gcJhjw61FW
        //// RJDLLnJh0F53XcjrZLbOuhss99RCG1Pt0Yu6PPTrEZ8b9XhVVJpQyiaUD3c00zbn
        public static async Task Main(string[] args)
        {

            //var order2 = await HelperHttpApi.order2();
            //var account = await HelperHttpApi.balance();

            //var sonuc = await Helper.marginAl();
            //var rateLimit = await Helper.wsTest();

            //Console.WriteLine(account);
          

            var connectionString = "Server=.;Database=coins;User Id=cengiz;Password=R45ty871;MultipleActiveResultSets=true;TrustServerCertificate=True";
            var queue = new ConcurrentQueue<int>();
            var processedSet = new HashSet<int>(); // İşlenmiş kayıtları tutmak için bir set oluştur
            processedSet.Add(0);
            async Task ProcessData()
            {
                while (true)
                {
                    int data;
                    // Tablodan bir kayıt al
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new SqlCommand("SELECT TOP 1 ID FROM PROCESS WHERE ID NOT IN (@Processed) ", connection))
                        {
                            try
                            {
                                command.Parameters.AddWithValue("@Processed", processedSet); // İşlenmiş kayıtları filtrele
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        data = reader.GetInt32(0); // Assuming YourColumn is string
                                    }
                                    else
                                    {
                                        await Task.Delay(100); // Tabloda işlenmemiş kayıt kalmadıysa biraz bekle
                                        continue; // Sonraki döngüye geç
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string error_msg = e.Message;
                                throw;
                            }
                        }
                    }

                    // Kuyruğa veriyi ekle
                    queue.Enqueue(data);
                    Console.WriteLine("Veri eklendi: " + data);
                }
            }

            async Task ProcessQueue()
            {
                while (true)
                {
                    if (queue.TryDequeue(out var data))
                    {
                        try
                        {
                            // Veriyi işle
                            Console.WriteLine("Veri işlendi: " + data);

                            // İşlenmiş kayıtları sete ekle
                            processedSet.Add(data);
                        }
                        catch (Exception e )
                        {
                            Console.WriteLine("HATA : " + e.Message);
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            }

            var processDataTask = Task.Run(ProcessData);
            var processQueueTask = Task.Run(ProcessQueue);

            await Task.WhenAll(processDataTask, processQueueTask);

            Console.ReadLine();
        }



      


    }
}