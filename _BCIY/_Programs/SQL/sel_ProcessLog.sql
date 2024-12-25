SELECT top 100 [Id]
      ,[ProcessId]
      ,[ReqPath]
      ,[ReqBody]
      ,[ResBody]
      ,[LogType]
      ,[InsertDateTime]
  FROM [coins].[dbo].[ProcessLog]
  WHERE 1=1
--AND [ProcessId] IN (SELECT ID FROM [PROCESS] WHERE SYMBOL='HOTUSDT.P' AND STATUS<>'CANCELED')
-- AND Logtype<>'PozisyonKontrol'
--  AND ProcessId = 896
   order by 1 desc