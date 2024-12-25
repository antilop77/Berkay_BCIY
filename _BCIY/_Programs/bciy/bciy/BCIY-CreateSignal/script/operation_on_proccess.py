from db import insert, select
from tradingview_ta import TA_Handler
import globals


def operation_on_proccess_function(i):
    coin_code = globals.coin_list[i][0]
    proccess_list = select.proccess_select(coin_code)
    update_close_price(i, proccess_list)
    pass


def update_close_price(i, proccess_list):
    coin_code = globals.coin_list[i][0]
    last_interval = None
    close_price = None

    for index in proccess_list:
        proccess_id = index[0]
        interval = index[1]
        last_close_price = index[2]
        price_unit = globals.coin_list[i][2]

        if last_interval != interval:
            last_interval = interval

            coin = TA_Handler(
                symbol=coin_code,
                screener="crypto",
                exchange="BINANCE",
                interval=interval,
            )

            coin_data = coin.get_analysis()

            precision_cls = len(price_unit.split('.')[1])
            close_price = coin_data.indicators['VWMA']
            close_price = float(format(close_price, f'.{precision_cls}f'))

        if close_price != float(last_close_price) and close_price:
            insert.close_price_update(proccess_id, close_price, proccess_id)
