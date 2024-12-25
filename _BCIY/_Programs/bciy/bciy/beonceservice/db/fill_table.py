import pyodbc
import globals
import time

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


def emir_olusturma(coin_kodu, islem_tutari,islem_tipi, islem_sekli, olusturma_zamani, islem_zamani, acilis_tutari, kapanis_tutari, stop_tutari, emir_sonucu, sonuc_zamani, islem_zaman_turu, islem_adedi):
    cursor.execute("INSERT INTO Islemler(IslemKodu,CoinKodu,IslemTutari,IslemTipi,IslemSekli,OlusturmaZamani, EmirDurumu,IslemZamani,AcilisTutari,KapanisTutari,StopTutari,EmirSonucu,SonucZamani,IslemZamanTuru, IslemAdedi) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
                   ('0', coin_kodu, islem_tutari, islem_tipi, islem_sekli, olusturma_zamani, '2', islem_zamani, acilis_tutari, str(kapanis_tutari), str(stop_tutari), emir_sonucu, sonuc_zamani, islem_zaman_turu, islem_adedi))
    cursor.commit()


def coinleri_cek():
    globals.coin_liste = []
    globals.son_islem_zaman = []
    cursor.execute("SELECT CoinKodu, [1m %], [5m %], [15m %], [30m %], [1h %], [4h %], [1d %], [1m], [5m], [15m], [30m], [1h], [4h], [1d] FROM CoinListesi WHERE AktifDurumu = 1")
    coin_listesi = cursor.fetchall()
    for index in range(len(coin_listesi)):
        bir_coin_liste = [
            coin_listesi[index][0] + str('.P'),
            coin_listesi[index][1].replace(' ', ''),
            coin_listesi[index][2].replace(' ', ''),
            coin_listesi[index][3].replace(' ', ''),
            coin_listesi[index][4].replace(' ', ''),
            coin_listesi[index][5].replace(' ', ''),
            coin_listesi[index][6].replace(' ', ''),
            coin_listesi[index][7].replace(' ', '')
        ]
        coin_son_islem_zamani = [
            coin_listesi[index][8],
            coin_listesi[index][9],
            coin_listesi[index][10],
            coin_listesi[index][11],
            coin_listesi[index][12],
            coin_listesi[index][13],
            coin_listesi[index][14]
        ]
        globals.son_islem_zaman.append(coin_son_islem_zamani)
        globals.coin_liste.append(bir_coin_liste)


def coin_alim_saati_guncelleme(islem_zamani, coin_index, islem_birimi):
    if islem_birimi == '1m':
        cursor.execute("UPDATE CoinListesi SET [1m]='%s' WHERE CoinKodu='%s'" % (islem_zamani, coin_index))
    if islem_birimi == '5m':
        cursor.execute("UPDATE CoinListesi SET [5m]='%s' WHERE CoinKodu='%s'" % (islem_zamani, coin_index))
    if islem_birimi == '15m':
        cursor.execute("UPDATE CoinListesi SET [15m]='%s' WHERE CoinKodu='%s'" % (islem_zamani, coin_index))
    conn.commit()


def son_bes_islem_sonucu_kontrol():
    cursor.execute("SELECT TOP (5) EmirSonucu, SiraNo FROM Islemler WHERE EmirSonucu = 1 or EmirSonucu = 3 ORDER BY SiraNo desc")
    coin_listesi = cursor.fetchall()
    for kontrol in coin_listesi:
        if kontrol[1] > globals.son_duraklatma_sira_no:
            if kontrol[0] == 1:
                return
        else:
            return
    globals.son_duraklatma_sira_no = coin_listesi[0][1]
    print('------Son 5 emir zararlı olması sebebi ile 10 dakika beklemeye alınıyor!------')
    time.sleep(600)


def indikator_veri_kayit(coin_veri):
    # Başlangıç zamanını kaydedelim
    start_time = time.time()

    coin_kodu = coin_veri.symbol
    islem_zaman_birimi = coin_veri.interval
    gelen_veri_zamani = coin_veri.time

    indikator_values = [(coin_kodu, indikator_adi, indikator_degeri, islem_zaman_birimi, gelen_veri_zamani)
                        for indikator_adi, indikator_degeri in coin_veri.indicators.items()]

    cursor.executemany("INSERT INTO IndikatorVerileri(CoinKodu,IndikatorAdi,IndikatorDegeri,IslemZamanBirimi,GelenVeriZamani) VALUES(?,?,?,?,?)",
                       indikator_values)

    cursor.commit()

    # Bitiş zamanını kaydedelim
    end_time = time.time()

    # Geçen süreyi hesaplayalım
    elapsed_time = round(end_time - start_time, 2)

    print(coin_kodu, islem_zaman_birimi, "İşlem tamamlandı. Geçen süre:", elapsed_time, "saniye.")