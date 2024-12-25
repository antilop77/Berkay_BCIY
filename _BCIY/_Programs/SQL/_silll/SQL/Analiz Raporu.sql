DECLARE @raporzamani AS DATETIME =  GETDATE()-1

SELECT sum(Sonuc) as ToplamSonuc, count(*) as IslemMiktari, IndikatorAdi FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
where 
Sonuc is not Null 
and GelenVeriZamani >= @raporzamani
group by IndikatorAdi

-- İndikatör Sonuçları

SELECT 'RSI Başarısız' as 'Kategori', count(*) as 'Sayı'  
FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'RSI' AND Sonuc < 0 AND GelenVeriZamani >= @raporzamani
UNION ALL
SELECT 'RSI Başarılı' as 'Kategori', count(*) as 'Sayı'
FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'RSI' AND Sonuc > 0 AND GelenVeriZamani >= @raporzamani
UNION ALL
SELECT 'RSI Sonuc' as 'Kategori', count(*) as 'Sayı'
FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'RSI' AND Sonuc != 0 AND GelenVeriZamani >= @raporzamani
select '%' + cast((SELECT count(*) FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK) WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'RSI' AND Sonuc > 0  AND GelenVeriZamani >= @raporzamani)
  *100/(SELECT count(*) FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK) WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'RSI' AND Sonuc != 0 AND GelenVeriZamani >= @raporzamani) as varchar)
	   as 'RSI Başarı %'
----------------------------------------------------------------------------------------
SELECT 'W.R Başarısız' as 'Kategori', count(*) as 'Sayı'  
FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'W.R' AND Sonuc < 0 AND GelenVeriZamani >= @raporzamani
UNION ALL
SELECT 'W.R Başarılı' as 'Kategori', count(*) as 'Sayı'
FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'W.R' AND Sonuc > 0 AND GelenVeriZamani >= @raporzamani
UNION ALL
SELECT 'W.R Sonuc Toplam' as 'Kategori', count(*) as 'Sayı'
FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK)
WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'W.R' AND Sonuc != 0 AND GelenVeriZamani >= @raporzamani
select '%' + cast((SELECT count(*) FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK) WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'W.R' AND Sonuc > 0 AND GelenVeriZamani >= @raporzamani)
  *100/(SELECT count(*) FROM [coins].[dbo].[IndikatorVerileri] with (NoLOCK) WHERE Sonuc IS NOT NULL AND IndikatorAdi = 'W.R' AND Sonuc != 0 AND GelenVeriZamani >= @raporzamani) as varchar)
	   as 'RSI Başarı %'



SELECT top 5 * FROM [coins].[dbo].[IndikatorVerileri] 
where Sonuc is not Null and IndikatorAdi = 'RSI' order by Sonuc


SELECT top 5 * FROM [coins].[dbo].[IndikatorVerileri] 
where Sonuc is not Null and IndikatorAdi = 'W.R' order by Sonuc