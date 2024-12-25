SELECT  [ID]
      ,[STATUS]
      ,[SYMBOL]
      ,[ORDER_SIDE]
      ,[ORDER_TYPE]
      ,[INTERVAL_TYPE]
      ,[QUANTITY]
      ,[OPEN_PRICE]
      ,[CLOSE_PRICE]
      ,[CLOSE_TYPE]
      ,[OPENED_PRICE]
      ,[EXECUTED_QUANTITY]
      ,[CLOSED_PRICE]
      ,[SIGNAL_TYPE]
      ,[BINANCE_ORDER_ID]
      ,[BINANCE_CLOSE_ORDER_ID]
      ,[BINANCE_STATUS]
      ,[INSERT_DATETIME]
      ,[UPDATE_DATETIME]
      ,[CLOSED_DATETIME]
  FROM [coins].[dbo].[PROCESS] 
  where 1=1
  and INSERT_DATETIME >= datetrunc(day, getdate())
  and SYMBOL = 'ADAUSDT.P' --'NEARUSDT.P' --
--and status IN('NEW', 'RENEW', 'OPENED')
--AND SYMBOL='AAVEUSDT.P'
--and BINANCE_CLOSE_ORDER_ID is null
--  and CLOSED_PRICE IS NOT N
  
  ORDER BY 1


  --SELECT 'Day', DATETRUNC(day, getdate())


