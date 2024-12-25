import pyodbc

from config import CONFIG

driver = CONFIG['db']['DRIVER']
server = CONFIG['db']['SERVER']
database = CONFIG['db']['NAME']
db_id = CONFIG['db']['USERNAME']
password = CONFIG['db']['PASSWORD']

conn = pyodbc.connect('DRIVER=' + driver + '; \
                           SERVER=' + server + '; \
                           DATABASE=' + database + ';\
                           UID=' + db_id + ';\
                           PWD=' + password + ';\
                           ')
cursor = conn.cursor()


def rsi_15m_select(coin_kodu):
    query = '''
                SELECT ID, IndikatorDegeri, GelenVeriZamani
                FROM (
                    SELECT *,
                           ROW_NUMBER() OVER(PARTITION BY DATEPART(year, gelenverizamani), DATEPART(month, gelenverizamani), DATEPART(day, gelenverizamani), DATEPART(hour, gelenverizamani), DATEPART(minute, gelenverizamani), IndikatorAdi ORDER BY IndikatorDegeri) as RowNum
                    FROM IndikatorVerileri with (NOLOCK)
                    WHERE CoinKodu=? 
                        AND IndikatorAdi in ('RSI') 
                        AND IslemZamanBirimi='15m'
                        AND GelenVeriZamani <= DATEADD(hour, -4, GETDATE()) -- Şu andan 4 saat ve daha öncesine ait verileri al
                ) AS SubQuery
                WHERE RowNum = 1 and Sonuc is null
            '''
    parameters = coin_kodu

    cursor.execute(query, parameters)

    coin_sonuc = cursor.fetchall()
    return coin_sonuc


def close_1m_select(gelen_veri_zamani, coin_kodu):
    query = '''
            SELECT IndikatorDegeri, GelenVeriZamani
            FROM IndikatorVerileri with (NOLOCK)
            WHERE CoinKodu=? and IndikatorAdi='close' and GelenVeriZamani>?
        '''
    parameters = (coin_kodu, gelen_veri_zamani)

    cursor.execute(query, parameters)

    kapanis_degerleri = cursor.fetchall()

    return kapanis_degerleri


def rsi_close_veri_select(gelen_veri_zamani, coin_kodu):
    query = '''
                SELECT top 1 IndikatorDegeri
                FROM (
                    SELECT *,
                           ROW_NUMBER() OVER(PARTITION BY DATEPART(year, gelenverizamani), DATEPART(month, gelenverizamani), DATEPART(day, gelenverizamani), DATEPART(hour, gelenverizamani), DATEPART(minute, gelenverizamani), IndikatorAdi ORDER BY IndikatorDegeri) as RowNum
                    FROM IndikatorVerileri with (NOLOCK)
                    WHERE CoinKodu=? AND IndikatorAdi = 'close' AND IslemZamanBirimi='15m' and GelenVeriZamani=?
                ) AS SubQuery
                WHERE RowNum = 1
            '''
    parameters = (coin_kodu, gelen_veri_zamani)

    cursor.execute(query, parameters)

    kapanis_degerleri = cursor.fetchone()[0]

    return kapanis_degerleri


def aktif_coinler():
    cursor.execute("SELECT CoinKodu FROM CoinListesi WHERE AktifDurumu = 1")
    coin_listesi = cursor.fetchall()
    return coin_listesi
