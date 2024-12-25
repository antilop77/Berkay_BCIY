import datetime
from db import select, update


def coin_analysis(thread_num, coin_code, time_unit, indicator):
    now_time = datetime.datetime.now()
    try:

        if indicator == 'RSI':
            rsi_return_list = select.rsi_data_analysis(coin_code, time_unit)
            for rsi_data in rsi_return_list:
                pass
        elif indicator == 'W.R':
            select.wr_data_analysis(coin_code, time_unit)
        elif indicator == 'BB':
            select.rsi_data_analysis(coin_code, time_unit)

        print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veri tabanina basarli kayit edildi. Zaman birimi =", indicator)

    except Exception as e:
        print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} hata alindi. Hata =", e, " Zaman birimi =", indicator)
