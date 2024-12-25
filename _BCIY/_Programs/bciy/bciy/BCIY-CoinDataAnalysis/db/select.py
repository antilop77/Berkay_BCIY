from db import connection, update
from datetime import datetime, timedelta
import config


def withdraw_coins():
    cursor = connection.cursor_function()

    try:
        cursor.execute("SELECT CoinKodu FROM CoinListesi WHERE AktifDurumu = 1")
        coin_listesi = cursor.fetchall()
        return coin_listesi
    except Exception as e:
        print(e)
        print("Coin verileri alinamadi.")
        return None


def rsi_data_analysis(coin_code, time_unit):
    cursor = connection.cursor_function()

    bugun = datetime.now()
    baslangic_zamani = bugun - timedelta(days=config.CONFIG['parameter']['ANALYSIS_PERIOD_DAYS'])
    rsi_min = config.CONFIG['parameter']['RSI_MIN']
    rsi_max = config.CONFIG['parameter']['RSI_MAX']
    vwma_percentage_of_approach = config.CONFIG['parameter']['VWMA_PERCENTAGE_OF_UNIT']

    try:
        datetime1 = datetime.now()
        # RSI[1] yerine RSI konulacak 24.04.2024 veriler guncellendi. 01.05.2024 de guncellenmesi gerekecek.
        query_signal = '''SELECT ID, IndikatorDegeri, GelenVeriZamani FROM IndikatorVerileri with (NOLOCK) 
                            WHERE 1=1  
                            AND IndikatorAdi='RSI'
                            AND CoinKodu= ?
                            AND IslemZamanBirimi= ?
                            AND GelenVeriZamani >= ? 
                            AND (IndikatorDegeri <= ? or  IndikatorDegeri >= ?) 
                            AND SONUC is Null
                            '''

        parameters = (coin_code, time_unit, baslangic_zamani, rsi_min, rsi_max)
        cursor.execute(query_signal, parameters)
        rsi_sonuc = cursor.fetchall()

        datetime2 = datetime.now()
        print(datetime2 - datetime1, "<----- rsi_sonuc select suresi ")

        if rsi_sonuc:
            for signal in rsi_sonuc:

                indikator_id = signal[0]
                indicator_deger = signal[1]
                gelen_veri_zamani = signal[2]
                islem_tipi = None

                if indicator_deger >= 70:
                    islem_tipi = 'SELL'
                elif indicator_deger <= 30:
                    islem_tipi = 'BUY'

                rsi_sonuc_kontrol_listesi = rsi_indikator_verileri(coin_code, gelen_veri_zamani)

                datetime3 = datetime.now()
                print(datetime3 - datetime2, "<----- rsi_sonuc_kontrol_listesi select suresi ")

                if rsi_sonuc_kontrol_listesi:
                    close = None
                    dusuk_sayisi = 1
                    yuksek_sayisi = 1
                    vwma = None
                    dusuk_deger = None
                    yuksek_deger = None

                    for rsi_sonuc_kontrol in rsi_sonuc_kontrol_listesi:
                        rsi_sonuc_indikator_adi = rsi_sonuc_kontrol[0]
                        rsi_sonuc_indikator_degeri = rsi_sonuc_kontrol[1]
                        rsi_sonuc_indikator_gelenverizamani = rsi_sonuc_kontrol[2]
                        rsi_sonuc_indikator_islemzamanbirimi = rsi_sonuc_kontrol[3]

                        if rsi_sonuc_indikator_adi == 'close' and close is None:
                            close = float(rsi_sonuc_indikator_degeri)

                        elif rsi_sonuc_indikator_adi == 'low' and rsi_sonuc_indikator_islemzamanbirimi == '1m':
                            dusuk_deger = float(rsi_sonuc_indikator_degeri)
                            dusuk_sayisi = dusuk_sayisi + 1

                        elif rsi_sonuc_indikator_adi == 'high' and rsi_sonuc_indikator_islemzamanbirimi == '1m':
                            yuksek_deger = float(rsi_sonuc_indikator_degeri)
                            yuksek_sayisi = yuksek_sayisi + 1

                        elif rsi_sonuc_indikator_adi == 'VWMA' and rsi_sonuc_indikator_islemzamanbirimi == time_unit:
                            vwma = float(rsi_sonuc_indikator_degeri)

                        if dusuk_sayisi % 2 == 0 and yuksek_sayisi % 2 == 0:
                            if vwma and dusuk_deger and yuksek_deger:
                                dusuk_deger = (vwma - dusuk_deger) * vwma_percentage_of_approach + dusuk_deger
                                yuksek_deger = (yuksek_deger - vwma) * vwma_percentage_of_approach + yuksek_deger

                                if dusuk_deger <= vwma <= yuksek_deger:
                                    if islem_tipi == 'SELL':
                                        print("Satıs islemi Acildi.")
                                        sonuc = (float((close - vwma)) / close) * 100
                                        update.indikator_id_update(indikator_id, sonuc)
                                        break
                                    elif islem_tipi == 'BUY':
                                        print("Alıs islemi Acildi.")
                                        sonuc = (float((vwma - close)) / close) * 100
                                        update.indikator_id_update(indikator_id, sonuc)
                                        break

                        else:
                            continue
        else:
            pass

        return rsi_sonuc
    except Exception as e:
        print(e)
        print("RSI verileri alinamadi.")
        return None


def rsi_indikator_verileri(coin_code, gelen_veri_zamani):

    cursor = connection.cursor_function()

    gelen_veri_zamani_str = gelen_veri_zamani.strftime('%Y-%m-%d %H:%M:%S')

    sql = ("select * from (SELECT IndikatorAdi, IndikatorDegeri, GelenVeriZamani, IslemZamanBirimi "
            "FROM IndikatorVerileri with (NOLOCK) "
            "WHERE CoinKodu = ? "
            "AND ( IndikatorAdi = 'high' or IndikatorAdi = 'low' or IndikatorAdi = 'VWMA' or IndikatorAdi = 'close' ) "
            "AND (IslemZamanBirimi = '1m' )"
            "AND GelenVeriZamani >= CONVERT(datetime, ?, 120) "
           "UNION ALL "
           "SELECT IndikatorAdi, IndikatorDegeri, GelenVeriZamani, IslemZamanBirimi "
           "FROM IndikatorVerileri with (NOLOCK) "
           "WHERE CoinKodu = ? "
           "AND ( IndikatorAdi = 'high' or IndikatorAdi = 'low' or IndikatorAdi = 'VWMA' or IndikatorAdi = 'close' ) "
           "AND (IslemZamanBirimi = '15m' )"
           "AND GelenVeriZamani >= CONVERT(datetime, ?, 120) ) xx ORDER BY GelenVeriZamani ")

    cursor.execute(sql, (coin_code, gelen_veri_zamani_str, coin_code, gelen_veri_zamani_str))

    rsi_sonuc_kontrol_listesi = cursor.fetchall()
    return rsi_sonuc_kontrol_listesi


def wr_data_analysis(coin_code, time_unit):
    cursor = connection.cursor_function()

    bugun = datetime.now()
    baslangic_zamani = bugun - timedelta(days=config.CONFIG['parameter']['ANALYSIS_PERIOD_DAYS'])
    wr_min = config.CONFIG['parameter']['WR_MIN']
    wr_max = config.CONFIG['parameter']['WR_MAX']
    vwma_percentage_of_approach = config.CONFIG['parameter']['VWMA_PERCENTAGE_OF_UNIT']

    try:
        datetime1 = datetime.now()
        query_signal = '''SELECT ID, IndikatorDegeri, GelenVeriZamani FROM IndikatorVerileri 
                            WHERE 1=1 
                            AND IndikatorAdi='W.R'
                            AND CoinKodu= ?
                            AND IslemZamanBirimi= ?
                            AND GelenVeriZamani >= ? 
                            AND (IndikatorDegeri <= ? or  IndikatorDegeri >= ?) 
                            AND SONUC is Null
                            '''

        parameters = (coin_code, time_unit, baslangic_zamani, wr_min, wr_max)
        cursor.execute(query_signal, parameters)
        rsi_sonuc = cursor.fetchall()

        datetime2 = datetime.now()
        print(datetime2 - datetime1, "<----- wr_sonuc select suresi ")

        if rsi_sonuc:
            for signal in rsi_sonuc:

                indikator_id = signal[0]
                indicator_deger = signal[1]
                gelen_veri_zamani = signal[2]
                islem_tipi = None

                if indicator_deger >= -20:
                    islem_tipi = 'SELL'
                elif indicator_deger <= -80:
                    islem_tipi = 'BUY'

                rsi_sonuc_kontrol_listesi = rsi_indikator_verileri(coin_code, gelen_veri_zamani)

                datetime3 = datetime.now()
                print(datetime3 - datetime2, "<----- wr_sonuc_kontrol_listesi select suresi ")

                if rsi_sonuc_kontrol_listesi:
                    close = None
                    dusuk_sayisi = 1
                    yuksek_sayisi = 1
                    vwma = None
                    dusuk_deger = None
                    yuksek_deger = None

                    for rsi_sonuc_kontrol in rsi_sonuc_kontrol_listesi:
                        wr_sonuc_indikator_adi = rsi_sonuc_kontrol[0]
                        wr_sonuc_indikator_degeri = rsi_sonuc_kontrol[1]
                        wr_sonuc_indikator_gelenverizamani = rsi_sonuc_kontrol[2]
                        wr_sonuc_indikator_islemzamanbirimi = rsi_sonuc_kontrol[3]

                        if wr_sonuc_indikator_adi == 'close' and close is None:
                            close = float(wr_sonuc_indikator_degeri)

                        elif wr_sonuc_indikator_adi == 'low' and wr_sonuc_indikator_islemzamanbirimi == '1m':
                            dusuk_deger = float(wr_sonuc_indikator_degeri)
                            dusuk_sayisi = dusuk_sayisi + 1

                        elif wr_sonuc_indikator_adi == 'high' and wr_sonuc_indikator_islemzamanbirimi == '1m':
                            yuksek_deger = float(wr_sonuc_indikator_degeri)
                            yuksek_sayisi = yuksek_sayisi + 1

                        elif wr_sonuc_indikator_adi == 'VWMA' and wr_sonuc_indikator_islemzamanbirimi == time_unit:
                            vwma = float(wr_sonuc_indikator_degeri)

                        if dusuk_sayisi % 2 == 0 and yuksek_sayisi % 2 == 0:
                            if vwma and dusuk_deger and yuksek_deger:
                                dusuk_deger = (dusuk_deger - vwma) * vwma_percentage_of_approach + dusuk_deger
                                yuksek_deger = (vwma - yuksek_deger) * vwma_percentage_of_approach + yuksek_deger

                                if dusuk_deger <= vwma <= yuksek_deger:
                                    if islem_tipi == 'SELL':
                                        sonuc = (float((close - vwma)) / close) * 100
                                        print("Satıs islemi Acildi.", sonuc)
                                        update.indikator_id_update(indikator_id, sonuc)
                                        break
                                    elif islem_tipi == 'BUY':
                                        sonuc = (float((vwma - close)) / close) * 100
                                        print("Alıs islemi Acildi.", sonuc)
                                        update.indikator_id_update(indikator_id, sonuc)
                                        break

                        else:
                            continue
        else:
            pass

        return rsi_sonuc
    except Exception as e:
        print(e)
        print("RSI verileri alinamadi.")
        return None
