from db import connection


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
