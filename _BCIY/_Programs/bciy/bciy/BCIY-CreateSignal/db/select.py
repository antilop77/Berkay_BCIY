import datetime

import globals
from db import connection


def withdraw_coins():
    cursor = connection.cursor_function()

    try:
        cursor.execute("SELECT CoinKodu, MinMiktar, FiyatAdimi, RSI, WR, BB FROM CoinListesi with (NOLOCK) WHERE AktifDurumu = 1 and SinyalVermeAcKapa = 1")
        coin_listesi = cursor.fetchall()
        return coin_listesi
    except Exception as e:
        print(e)
        print("Coin verileri alinamadi.")
        return None


def get_parametrs():
    cursor = connection.cursor_function()

    try:
        cursor.execute("SELECT Kod, Deger FROM Parametreler")
        parametrs = cursor.fetchall()

        for x in parametrs:
            if 'IslemTutari' == x[0]:
                globals.order_price = x[1]

            elif 'SistemOnOFF' == x[0]:
                globals.system_on_off = x[1]

            elif 'RsiMax' == x[0]:
                globals.rsi_max_value = x[1]

            elif 'RsiMin' == x[0]:
                globals.rsi_min_value = x[1]

            elif 'WrMax' == x[0]:
                globals.wr_max_value = x[1]

            elif 'WrMin' == x[0]:
                globals.wr_min_value = x[1]

            elif 'BollingerAspect' == x[0]:
                globals.bollinger_aspect = x[1]

            elif 'CheckLastSignal' == x[0]:
                globals.check_last_signal_value = x[1]

            elif 'DistanceClosingValue' == x[0]:
                globals.distance_closing_value = x[1]

            elif 'HighPopularityMaxChange' == x[0]:
                globals.high_popularity_max_change = x[1]

    except Exception as e:
        print(e)
        print("Parametre verileri alinamadi.")
        return None


def proccess_select(coin_code):
    cursor = connection.cursor_function()

    now = datetime.datetime.now()
    now = now.strftime('%Y-%m-%d %H:%M:%S')

    try:
        query = "SELECT ID, INTERVAL_TYPE, CLOSE_PRICE FROM [PROCESS] with (NOLOCK) WHERE SYMBOL = ? and [STATUS] in ('OPENED', 'RENEW', 'PENDING', 'NEW') order by INTERVAL_TYPE"

        parameters = coin_code
        cursor.execute(query, parameters)
        proccess_list = cursor.fetchall()
        return proccess_list
    except Exception as e:
        print(e)
        print("Coin verileri alinamadi.")
        return None


def last_proccess_control(coin_code, processing_time, signal_type, processing_type):
    cursor = connection.cursor_function()

    now = datetime.datetime.now()
    now = now.strftime('%Y-%m-%d %H:%M:%S')

    try:
        query = (" SELECT * FROM PROCESS with (NOLOCK) "
                 " where SYMBOL = ? "
                 " and INTERVAL_TYPE = ? "
                 " and INSERT_DATETIME > DATEADD(MINUTE, - 2*(? + 5), GETDATE()) ")

        parameters = (coin_code, processing_time, processing_type)
        cursor.execute(query, parameters)
        last_proccess = cursor.fetchone()
        return last_proccess
    except Exception as e:
        print(e)
        print("Coin verileri alinamadi.")
        return None


def order_side_control(coin_code, order_side):
    cursor = connection.cursor_function()

    if order_side == 'BUY':
        new_order_side = 'SELL'
    else:
        new_order_side = 'BUY'

    try:
        query = "SELECT ID, INTERVAL_TYPE, CLOSE_PRICE FROM [PROCESS] with (NOLOCK) WHERE SYMBOL = ? and [STATUS] in ('OPENED', 'RENEW', 'PENDING', 'NEW')"

        parameters = coin_code
        cursor.execute(query, parameters)
        order_list = cursor.fetchall()
        return order_list
    except Exception as e:
        print(e)
        print("Coin verileri alinamadi.")
        return None
    pass

