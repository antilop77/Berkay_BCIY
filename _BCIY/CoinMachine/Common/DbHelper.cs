using Common.Dtos;
using Common.Model;
using Dapper;
using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Common
{
    public class DbHelper
    {
        private CoinsContext dbContext;

        private DbContextOptions<CoinsContext> GetAllOptions()
        {
            var optionBuilder = new DbContextOptionsBuilder<CoinsContext>();
            optionBuilder.UseSqlServer(AppSettings.ConnectionString);
            return optionBuilder.Options;
        }

        //GetAllActiveCoinList
        public List<CoinListesi> GetAllActiveCoinList()
        {
            using (dbContext = new CoinsContext(GetAllOptions()))
            {
                try
                {
                    var coinListesi = dbContext.CoinListesis.Where(x => x.AktifDurumu == 1).ToList();
                    if (coinListesi != null)
                        return coinListesi;
                    else
                        return new List<CoinListesi>();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Durum bakmaz
        /// </summary>
        /// <returns></returns>
        public List<CoinListesi> CoinListesiGetir()
        {
            using (dbContext = new CoinsContext(GetAllOptions()))
            {
                try
                {
                    var coinListesi = dbContext.CoinListesis.ToList();
                    if (coinListesi != null)
                        return coinListesi;
                    else
                        return new List<CoinListesi>();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //GetCoinDetay
        public async Task<CoinListesi> GetCoinDetay(string coinKodu)
        {
            using (dbContext = new CoinsContext(GetAllOptions()))
            {
                try
                {
                    var coin = dbContext.CoinListesis.Where(x => x.CoinKodu == coinKodu).FirstOrDefault();
                    if (coin != null)
                        return coin;
                    else
                        return new CoinListesi();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static async Task<List<PROCESS_DTO>> GuncellenecekProcessListesi()
        {
            try
            {
                List<PROCESS_DTO> processList = new List<PROCESS_DTO>();
                using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    connection.Open();

                    string Query = @" 
                                SELECT ID,STATUS,SYMBOL,ORDER_SIDE,ORDER_TYPE,INTERVAL_TYPE,QUANTITY,OPEN_PRICE,CLOSE_PRICE,CLOSE_PRICE,CLOSE_TYPE,OPENED_PRICE
                                      ,EXECUTED_QUANTITY
                                      ,SIGNAL_TYPE
                                      ,STOP_PRICE
                                      ,BINANCE_ORDER_ID
                                      ,EXECUTED_QUANTITY
                                      ,CALL_BACK_RATE
                                      ,BINANCE_CLOSE_ORDER_ID
                                      ,BINANCE_TRAILING_ORDER_ID
                                      ,BINANCE_TP_ORDER_ID
                                      ,BINANCE_SL_ORDER_ID
                                      ,BINANCE_STATUS
                                      ,INSERT_DATETIME
                                      ,UPDATE_DATETIME
                                      ,CLOSED_DATETIME
                                FROM PROCESS
                                WHERE 1=1 
                                AND [STATUS]='RENEW' OR
                                  (STATUS = 'OPENED') AND (ISNULL(BINANCE_CLOSE_ORDER_ID, N'') = '')
                                    OR
                                  (STATUS = 'OPENED') AND (ISNULL(BINANCE_TRAILING_ORDER_ID, N'') = '') 
                                order by ID ASC
                                ";

					/*
                      OR
                                  (STATUS = 'OPENED') AND (ISNULL(BINANCE_STOP_ORDER_ID, N'') = '') OR
                                  (STATUS = 'OPENED') AND (ISNULL(BINANCE_TP_ORDER_ID, N'') = '') OR
                                  (STATUS = 'OPENED') AND (ISNULL(BINANCE_SL_ORDER_ID, N'') = '')
                     */

					var sonuc = connection.Query(Query).ToList();

                    foreach (var item in sonuc)
                    {
                        PROCESS_DTO dto = new PROCESS_DTO();
                        dto.Id = Convert.ToInt32(item.ID);
                        dto.Status = item.STATUS;
                        dto.Symbol = item.SYMBOL;
                        dto.OrderSide = item.ORDER_SIDE;
                        dto.IntervalType = item.INTERVAL_TYPE;
                        dto.Quantity = item.QUANTITY;
                        dto.OpenPrice = item.OPEN_PRICE;
                        dto.StopPrice = item.STOP_PRICE;
                        dto.ClosePrice = item.CLOSE_PRICE;
                        dto.ClosedPrice = item.CLOSED_PRICE;
                        dto.CloseType = item.CLOSE_TYPE;
                        dto.OpenedPrice = item.OPENED_PRICE;
                        dto.ExecutedQuantity = item.EXECUTED_QUANTITY;
                        dto.CallBackRate = item.CALL_BACK_RATE;
                        dto.SignalType = item.SIGNAL_TYPE;
                        dto.BinanceOrderId = item.BINANCE_ORDER_ID;
                        dto.BinanceCloseOrderId = item.BINANCE_CLOSE_ORDER_ID;
                        dto.BinanceTrailingOrderId = item.BINANCE_TRAILING_ORDER_ID;
                        dto.BINANCE_TP_ORDER_ID = item.BINANCE_TP_ORDER_ID;
                        dto.BINANCE_SL_ORDER_ID = item.BINANCE_SL_ORDER_ID;
                        dto.BinanceStatus = item.BINANCE_STATUS;
                        dto.InsertDatetime = item.INSERT_DATETIME;
                        dto.UpdateDatetime = item.UPDATE_DATETIME;
                        dto.ClosedDatetime = item.CLOSED_DATETIME;
                        processList.Add(dto);
                    }

                    connection.Close();

                    return processList;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public async Task<List<PROCESS_DTO>> GuncellenecekPozisyonKontrolListesi()
        {
            try
            {
                List<PROCESS_DTO> processList = new List<PROCESS_DTO>();
                using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    connection.Open();

                    string Query = @" 
                                SELECT ID,STATUS,SYMBOL,ORDER_SIDE,ORDER_TYPE,INTERVAL_TYPE,QUANTITY,OPEN_PRICE,CLOSE_PRICE,CLOSED_PRICE,CLOSE_TYPE,OPENED_PRICE
                                      ,EXECUTED_QUANTITY
                                      ,SIGNAL_TYPE
                                      ,BINANCE_ORDER_ID
                                      ,BINANCE_CLOSE_ORDER_ID
                                      ,BINANCE_TP_ORDER_ID
                                      ,BINANCE_SL_ORDER_ID
                                      ,BINANCE_TRAILING_ORDER_ID
                                      ,STOP_PRICE
                                      ,CALL_BACK_RATE
                                      ,BINANCE_STATUS
                                      ,INSERT_DATETIME
                                      ,UPDATE_DATETIME
                                      ,CLOSED_DATETIME
                                FROM PROCESS
                                WHERE 1=1 
                                AND [STATUS] IN ('OPENED')
                                order by ID ASC
                                ";

                    var sonuc = connection.Query(Query).ToList();

                    foreach (var item in sonuc)
                    {
                        PROCESS_DTO dto = new PROCESS_DTO();
                        dto.Id = Convert.ToInt32(item.ID);
                        dto.Status = item.STATUS;
                        dto.Symbol = item.SYMBOL;
                        dto.OrderSide = item.ORDER_SIDE;
                        dto.OrderType = item.ORDER_TYPE;
                        dto.IntervalType = item.INTERVAL_TYPE;
                        dto.Quantity = item.QUANTITY;
                        dto.OpenPrice = item.OPEN_PRICE;
                        dto.StopPrice = item.STOP_PRICE;
                        dto.ClosePrice = item.CLOSE_PRICE;
                        dto.ClosedPrice = item.CLOSED_PRICE;
                        dto.CloseType = item.CLOSE_TYPE;
                        dto.OpenedPrice = item.OPENED_PRICE;
                        dto.ExecutedQuantity = item.EXECUTED_QUANTITY;
						dto.CallBackRate = item.CALL_BACK_RATE;
						dto.SignalType = item.SIGNAL_TYPE;
                        dto.BinanceOrderId = item.BINANCE_ORDER_ID;
                        dto.BinanceTrailingOrderId = item.BINANCE_TRAILING_ORDER_ID;
                        dto.BINANCE_TP_ORDER_ID = item.BINANCE_TP_ORDER_ID;
                        dto.BINANCE_SL_ORDER_ID = item.BINANCE_SL_ORDER_ID;
                        dto.BinanceCloseOrderId = item.BINANCE_CLOSE_ORDER_ID;
                        dto.BinanceStatus = item.BINANCE_STATUS;
                        dto.InsertDatetime = item.INSERT_DATETIME;
                        dto.UpdateDatetime = item.UPDATE_DATETIME;
                        dto.ClosedDatetime = item.CLOSED_DATETIME;
                        processList.Add(dto);
                    }

                    await connection.CloseAsync();

                    return processList;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public DateTime CoinVerileriBinanceSonIslemSaati()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    connection.Open();

                    string Query = @"SELECT max(GelenveriZamani) GelenveriZamani FROM CoinVerileriBinance ";
                    var gelenVeriZamani = connection.QueryFirstOrDefault<DateTime?>(Query);

                    
                    connection.Close();

                    return gelenVeriZamani.Value;
                }
            }
            catch (Exception e)
            {
                return DateTime.UtcNow;
            }
        }

        public async Task CoinVeriKaydet(string symbol, List<CoinVerileriBinance> coinVerisi)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    await connection.OpenAsync();

                    foreach (var item in coinVerisi)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@CoinKodu", symbol);
                        parameters.Add("@IslemZamanBirimi", item.IslemZamanBirimi);
                        parameters.Add("@OlusturmaZamani", item.OlusturmaZamani);
                        parameters.Add("@GelenVeriZamani", item.GelenVeriZamani);
                        parameters.Add("@open", item.Acilis);
                        parameters.Add("@high", item.Yuksek);
                        parameters.Add("@low", item.Dusuk);
                        parameters.Add("@close", item.Kapanis);
                        parameters.Add("@hacim", item.Hacim);

                        string insertQuery = @"INSERT INTO CoinVerileriBinance (CoinKodu, IslemZamanBirimi, OlusturmaZamani, GelenVeriZamani, Acilis, Yuksek, Dusuk, Kapanis , Hacim ) 
                                   VALUES (@CoinKodu, @IslemZamanBirimi, @OlusturmaZamani, @GelenVeriZamani, @open, @high, @low, @close, @hacim) ";
                        await connection.ExecuteAsync(insertQuery, parameters);
                    }


                    connection.Close();
                }
            }
            catch (Exception e)
            {
                string a = e.Message;
                throw;
            }

            //try
            //{
            //    using (IDbConnection db = new SqlConnection(AppSettings.ConnectionString))
            //    {
            //        string insertQuery = @"INSERT INTO CoinVerileriBinance (CoinKodu, IndikatorAdi, IndikatorDegeri, IslemZamanBirimi, OlusturmaZamani, GelenVeriZamani, Sonuc) 
            //                       VALUES (" + symbol + ", @IndikatorAdi, @IndikatorDegeri, @IslemZamanBirimi, @OlusturmaZamani, @GelenVeriZamani, @Sonuc)";
            //        await db.ExecuteAsync(insertQuery, coinVerisi);
            //    }
            //}
            //catch (Exception e)
            //{
            //    string s = e.Message;
            //}
        }

        public static async Task<int> CoinDurumUpdate(PROCESS_DTO process)
        {
            /* emir.BinanceCloseOrderId = kapatEmri.Result.orderId;
            emir.Status = "OPENED";
            emir.UpdateDatetime = DateTime.Now;
            */

            try
            {
                int cnt = 0;
                using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    await connection.OpenAsync();

                        var parameters = new DynamicParameters();
                        parameters.Add("@ProcessId", process.Id);
                        parameters.Add("@STATUS", process.Status);
                        parameters.Add("@BINANCE_CLOSE_ORDER_ID", process.BinanceCloseOrderId);

                        string insertQuery = @"UPDATE Process SET STATUS=@STATUS, BINANCE_CLOSE_ORDER_ID=@BINANCE_CLOSE_ORDER_ID, UPDATE_DATETIME=GETDATE() WHERE ID=@ProcessId ";
                        cnt = await connection.ExecuteAsync(insertQuery, parameters);
                    
                    connection.Close();
                }

                return cnt;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return -1;
            }
        }

		public static async Task<int> CoinTPDurumUpdate(PROCESS_DTO process)
		{
			/* emir.BinanceCloseOrderId = kapatEmri.Result.orderId;
            emir.Status = "OPENED";
            emir.UpdateDatetime = DateTime.Now;
            */

			try
			{
				int cnt = 0;
				using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
				{
					await connection.OpenAsync();

					var parameters = new DynamicParameters();
					parameters.Add("@ProcessId", process.Id);
					parameters.Add("@STATUS", process.Status);
					parameters.Add("@BINANCE_TP_ORDER_ID", process.BinanceCloseOrderId);

					string insertQuery = @"UPDATE Process SET STATUS=@STATUS, BINANCE_TP_ORDER_ID=@BINANCE_TP_ORDER_ID, UPDATE_DATETIME=GETDATE() WHERE ID=@ProcessId ";
					cnt = await connection.ExecuteAsync(insertQuery, parameters);

					connection.Close();
				}

				return cnt;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return -1;
			}
		}

		public static async Task<int> CoinSLDurumUpdate(PROCESS_DTO process)
		{
			/* emir.BinanceCloseOrderId = kapatEmri.Result.orderId;
            emir.Status = "OPENED";
            emir.UpdateDatetime = DateTime.Now;
            */

			try
			{
				int cnt = 0;
				using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
				{
					await connection.OpenAsync();

					var parameters = new DynamicParameters();
					parameters.Add("@ProcessId", process.Id);
					parameters.Add("@STATUS", process.Status);
					parameters.Add("@BINANCE_SL_ORDER_ID", process.BinanceCloseOrderId);

					string insertQuery = @"UPDATE Process SET STATUS=@STATUS, BINANCE_SL_ORDER_ID=@BINANCE_SL_ORDER_ID, UPDATE_DATETIME=GETDATE() WHERE ID=@ProcessId ";
					cnt = await connection.ExecuteAsync(insertQuery, parameters);

					connection.Close();
				}

				return cnt;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return -1;
			}
		}

		public static async Task<int> CoinTRAILINGDurumUpdate(PROCESS_DTO process)
		{
			/* emir.BinanceCloseOrderId = kapatEmri.Result.orderId;
            emir.Status = "OPENED";
            emir.UpdateDatetime = DateTime.Now;
            */

			try
			{
				int cnt = 0;
				using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
				{
					await connection.OpenAsync();

					var parameters = new DynamicParameters();
					parameters.Add("@ProcessId", process.Id);
					parameters.Add("@STATUS", process.Status);
					parameters.Add("@BINANCE_TRAILING_ORDER_ID", process.BinanceTrailingOrderId);

					string insertQuery = @"UPDATE Process SET STATUS=@STATUS, BINANCE_TRAILING_ORDER_ID=@BINANCE_TRAILING_ORDER_ID, UPDATE_DATETIME=GETDATE() WHERE ID=@ProcessId ";
					cnt = await connection.ExecuteAsync(insertQuery, parameters);

					connection.Close();
				}

				return cnt;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return -1;
			}
		}

		public async Task<int> CoinPozisyonDurumUpdate(PROCESS_DTO process)
        {
            try
            {
                int cnt = 0;
                using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@ProcessId", process.Id);
                    parameters.Add("@STATUS", process.Status);
                    parameters.Add("@ClosedPrice", process.ClosedPrice);

                    string insertQuery = @"UPDATE Process SET STATUS=@STATUS, CLOSED_PRICE=@ClosedPrice, CLOSED_DATETIME=GETDATE() WHERE ID=@ProcessId ";
                    cnt = await connection.ExecuteAsync(insertQuery, parameters);

                    connection.Close();
                }

                return cnt;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return -1;
            }
        }

        public bool ProcessLogYaz(int ProcessId, string ReqPath, string ReqBody, string ResBody, string LogType)
        {
            using (dbContext = new CoinsContext(GetAllOptions()))
            {
                try
                {
                    ProcessLog processLog = new ProcessLog();
                    processLog.ProcessId = ProcessId;
                    processLog.ReqPath = ReqPath;
                    processLog.ReqBody = ReqBody;
                    processLog.ResBody = ResBody;
                    processLog.InsertDateTime = DateTime.Now;
                    processLog.LogType = LogType;
                    dbContext.ProcessLogs.Add(processLog);
                    dbContext.SaveChanges();

                    return false;
                }
                catch (Exception e)
                {
                    Log.Error("ProcessLogYaz LOG YAZILIRKEN HATA" + ProcessId + " -" + e.Message);
                    return true;
                }
            }
        }

        public async Task<bool> ProcessLogYazAsync(int ProcessId, string ReqPath, string ReqBody, string ResBody, string LogType)
        {
            using (dbContext = new CoinsContext(GetAllOptions()))
            {
                try
                {
                    ProcessLog processLog = new ProcessLog();
                    processLog.ProcessId = ProcessId;
                    processLog.ReqPath = ReqPath;
                    processLog.ReqBody = ReqBody;
                    processLog.ResBody = ResBody;
                    processLog.InsertDateTime = DateTime.Now;
                    processLog.LogType = LogType;
                    dbContext.ProcessLogs.Add(processLog);
                    int cnt =  await dbContext.SaveChangesAsync();

                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("ProcessLogYazAsync LOG YAZILIRKEN HATA" + ProcessId + " -" + e.Message);
                    return false;
                }
            }
        }

        public async Task<bool> ProcessLogYazAsyncADO(int ProcessId, string ReqPath, string ReqBody, string ResBody, string LogType)
        {
            string insertQuery = @"INSERT INTO ProcessLog (ProcessId, ReqPath, ReqBody, ResBody, InsertDateTime, LogType) 
                           VALUES (@ProcessId, @ReqPath, @ReqBody, @ResBody, GETDATE(), @LogType)";

            using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@ProcessId", ProcessId);
                command.Parameters.AddWithValue("@ReqPath", ReqPath);
                command.Parameters.AddWithValue("@ReqBody", ReqBody);
                command.Parameters.AddWithValue("@ResBody", ResBody);
                command.Parameters.AddWithValue("@LogType", LogType);

                try
                {
                    connection.Open();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ProcessLogYazAsync LOG YAZILIRKEN HATA: " + ProcessId + " - " + e.Message);
                    return false;
                }
            }
        }
    }
}
