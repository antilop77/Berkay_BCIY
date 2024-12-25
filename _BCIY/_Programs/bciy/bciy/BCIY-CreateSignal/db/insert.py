import datetime
import time
from db import connection


def open_order_signal(coin_code, order_side, order_type, processing_time, quantity, open_price, close_price, signal_type, indicator_value, stop_price, call_back_rate):
    cursor = connection.cursor_function()

    now = datetime.datetime.now()

    try:
        insert_query = """
            INSERT INTO [coins].[dbo].[PROCESS] 
            ([STATUS], [SYMBOL], [ORDER_SIDE], [ORDER_TYPE], [INTERVAL_TYPE], [QUANTITY], [OPEN_PRICE], [CLOSE_PRICE], [SIGNAL_TYPE], [INSERT_DATETIME], [INDICATOR_VALUE], [STOP_PRICE], [CALL_BACK_RATE]) 
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """

        cursor.execute(insert_query, ('NEW', coin_code, order_side, order_type, processing_time, quantity, open_price, close_price, signal_type, now.strftime('%Y-%m-%d %H:%M:%S'), indicator_value, stop_price, call_back_rate))
        cursor.commit()
        print("Veri başarıyla eklendi.")

    except Exception as e:
        print("Veri eklenirken hata oluştu:", str(e))
        cursor.rollback()

    finally:
        cursor.close()


def close_price_update(coin_code, close_price, proccess_id):
    cursor = connection.cursor_function()
    now = datetime.datetime.now()

    try:
        update_query = """
            UPDATE PROCESS 
            SET STATUS = 'RENEW', CLOSE_PRICE = ?, UPDATE_DATETIME = ? 
            WHERE ID = ? AND STATUS IN ('OPENED', 'RENEW');

            UPDATE PROCESS 
            SET CLOSE_PRICE = ?, UPDATE_DATETIME = ? 
            WHERE ID = ? AND STATUS IN ('PENDING', 'NEW');
        """

        cursor.execute(update_query, (close_price, now.strftime('%Y-%m-%d %H:%M:%S'), proccess_id, close_price, now.strftime('%Y-%m-%d %H:%M:%S'), proccess_id))
        cursor.commit()
        print("Veri güncellendi.")

    except Exception as e:
        print("Veri güncellenirken hata oluştu:", str(e))
        cursor.rollback()

    finally:
        cursor.close()
