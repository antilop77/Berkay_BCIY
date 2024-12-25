from tradingview_ta import TA_Handler


def get_data(coin_code, processing_time):
    try:
        coin_data = TA_Handler(
            symbol=coin_code,
            screener="crypto",
            exchange="BINANCE",
            interval=processing_time,

        )
        return coin_data.get_analysis()
    except Exception as e:
        print(e)
