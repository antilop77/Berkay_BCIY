import db.select_table
import db.fill_table
from datetime import datetime, timedelta


def main():
    try:
        coin_list = db.select_table.aktif_coinler()

        for coin_kodu in coin_list:

            coin_kodu = coin_kodu[0] + str('.P')
            print(coin_kodu, "<---- İşlem Başladı")
            coin_sonuc = db.select_table.rsi_15m_select(coin_kodu)
            rsi_15m_kontrol_yuzde = 1.5

            for indikator_veri in coin_sonuc:
                indikator_veri_id = indikator_veri[0]
                indikator_deger = indikator_veri[1]
                gelen_veri_zamani = indikator_veri[2]
                sure_siniri = gelen_veri_zamani + timedelta(hours=4)
                kapanis_degerleri = db.select_table.close_1m_select(gelen_veri_zamani=gelen_veri_zamani, sure_siniri=sure_siniri, coin_kodu=coin_kodu)
                close_deger = db.select_table.rsi_close_veri_select(gelen_veri_zamani=gelen_veri_zamani, coin_kodu=coin_kodu)

                indikator_sonucu = 0

                for deger_kontrol in kapanis_degerleri:
                    kontrol_verisi = deger_kontrol[0]
                    if float(close_deger)*(float(100)+rsi_15m_kontrol_yuzde)/100 < kontrol_verisi:
                        indikator_sonucu = 1
                        print("yükseldi", indikator_veri_id, "indikatör değeri = ", indikator_deger)
                        db.fill_table.indikator_sonuc_update(indikator_veri_id=indikator_veri_id, indikator_sonucu=indikator_sonucu)
                        break

                    elif float(close_deger)*(float(100)-rsi_15m_kontrol_yuzde)/100 > kontrol_verisi:
                        indikator_sonucu = 2
                        print("düştü", indikator_veri_id, "indikatör değeri = ", indikator_deger)
                        db.fill_table.indikator_sonuc_update(indikator_veri_id=indikator_veri_id, indikator_sonucu=indikator_sonucu)
                        break

                if indikator_sonucu == 0:
                    db.fill_table.indikator_sonuc_update(indikator_veri_id=indikator_veri_id, indikator_sonucu=indikator_sonucu)
                    print("nötr kaldı", indikator_veri_id, "indikatör değeri = ", indikator_deger)

    except Exception as e:
        print(e)


while True:
    try:
        main()
    except Exception as e:
        print(e)
