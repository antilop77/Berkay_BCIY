DECLARE @raporzamani AS DATETIME =  GETDATE()-1

SELECT sum(Sonuc) as ToplamSonuc, count(*) as IslemMiktari, IndikatorAdi FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where 
Sonuc is not Null 
and GelenVeriZamani >= @raporzamani
group by IndikatorAdi

/*
SELECT sum(Sonuc) as ToplamSonuc, count(*) as IslemMiktari, IndikatorAdi FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where 
Sonuc is not Null 
and IndikatorAdi = 'W.R' 
and (IndikatorDegeri > -10 or IndikatorDegeri < -90) 
and GelenVeriZamani >= @raporzamani
group by IndikatorAdi
*/

-- İndikatör Sonuçları

SELECT count(*) as 'RSI Başarısız'  FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where Sonuc is not Null and IndikatorAdi = 'RSI' and Sonuc < 0 and GelenVeriZamani >= @raporzamani

SELECT count(*) as 'RSI Başarılı' FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where Sonuc is not Null and IndikatorAdi = 'RSI' and Sonuc > 0 and GelenVeriZamani >= @raporzamani

SELECT count(*) as 'RSI Sonuc' FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where Sonuc is not Null and IndikatorAdi = 'RSI' and Sonuc != 0 and GelenVeriZamani >= @raporzamani

----------------------------------------------------------------------------------------

SELECT count(*) as 'W.R Başarısız' FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where Sonuc is not Null and IndikatorAdi = 'W.R' and Sonuc < 0 and GelenVeriZamani >= @raporzamani

SELECT count(*) as 'W.R Başarılı' FROM   [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where Sonuc is not Null and IndikatorAdi = 'W.R' and Sonuc > 0 and GelenVeriZamani >= @raporzamani

SELECT count(*) as 'W.R Sonuc' FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where Sonuc is not Null and IndikatorAdi = 'W.R' and Sonuc != 0 and GelenVeriZamani >= @raporzamani

