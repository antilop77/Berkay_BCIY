import datetime
import time

from tradingview_ta import Interval
import globals


def yuzde_kontrol(coin_index):
    if globals.coin_kontrol_zamani == '1m':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][1]

    elif globals.coin_kontrol_zamani == '5m':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][2]

    elif globals.coin_kontrol_zamani == '15m':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][3]

    elif globals.coin_kontrol_zamani == '30m':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][4]

    elif globals.coin_kontrol_zamani == '1h':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][5]

    elif globals.coin_kontrol_zamani == '4h':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][6]

    elif globals.coin_kontrol_zamani == '1d':
        globals.kar_yuzdesi = globals.coin_liste[coin_index][7]


def rsi_degerleri_guncelle():
    st = time.time()
    suanki_zaman_1 = datetime.datetime.fromtimestamp(st)
    for index in range(len(globals.coin_liste)):
        if globals.son_islem_zaman[index][0] is None or globals.son_islem_zaman[index][0] + datetime.timedelta(minutes=10) > suanki_zaman_1:
            globals.max_rsi = 74
            globals.min_rsi = 26
            print(globals.max_rsi,"max rsi")
            print(globals.min_rsi,"min rsi")
            return
    globals.max_rsi = 72
    globals.min_rsi = 28
    print(globals.max_rsi,"max rsi")
    print(globals.min_rsi,"min rsi")
