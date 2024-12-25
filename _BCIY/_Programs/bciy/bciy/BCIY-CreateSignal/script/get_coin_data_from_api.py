from tradingview_ta import TA_Handler
from db import insert, select
import globals
import requests


def get_data(i, processing_time):
    coin_code = globals.coin_list[i][0]
    min_unit = globals.coin_list[i][1]
    price_unit = globals.coin_list[i][2]
    rsi = globals.coin_list[i][3]
    wr = globals.coin_list[i][4]
    bb = globals.coin_list[i][5]
    rsi_signal = 0

    if coin_code == 'BTCUSDT.P':
        high_popularity_indicator(processing_time)

    else:
        try:
            coin = TA_Handler(
                symbol=coin_code,
                screener="crypto",
                exchange="BINANCE",
                interval=processing_time,
            )
            coin_data = coin.get_analysis()

            close_value = float(coin_data.indicators['close'])

            if rsi:
                result = rsi_control(coin_data, processing_time, i)

                if result:
                    signal_type = 'RSI'
                    order_side = result
                    order_type = 'MARKET'
                    rsi_signal = 1

                    order_control = select.order_side_control(coin_code, order_side)

                    if not order_control:

                        opened_function(min_unit, close_value, price_unit, coin_data, coin_code, order_side, signal_type, order_type, processing_time)

            if wr and rsi_signal != 1:
                result = wr_control(coin_data, processing_time, i)

                if result:
                    signal_type = 'W.R'
                    order_side = result
                    order_type = 'MARKET'

                    order_control = select.order_side_control(coin_code, order_side)

                    if not order_control:

                        opened_function(min_unit, close_value, price_unit, coin_data, coin_code, order_side, signal_type, order_type, processing_time)

        except Exception as e:
            print(e)


def rsi_control(coin_data, processing_time, i):
    rsi_value = coin_data.indicators['RSI']
    signal_type = 'RSI'

    if rsi_value < float(globals.rsi_min_value):
        bb_control = signal_control_by_bb(coin_data)

        last_signal_control = check_last_signal(i, processing_time, signal_type)

        if bb_control and last_signal_control:
            print("RSI Alis sinyali geldi.")
            order_side = 'BUY'
            return order_side

    if rsi_value > float(globals.rsi_max_value):
        bb_control = signal_control_by_bb(coin_data)

        last_signal_control = check_last_signal(i, processing_time, signal_type)

        if bb_control and last_signal_control:
            print("RSI Satis sinyali geldi.")
            order_side = 'SELL'
            return order_side

    return False


def wr_control(coin_data, processing_time, i):
    wr_value = coin_data.indicators['W.R']
    signal_type = 'W.R'

    if wr_value < float(globals.wr_min_value):
        bb_control = signal_control_by_bb(coin_data)

        last_signal_control = check_last_signal(i, processing_time, signal_type)

        if bb_control and last_signal_control:
            print("W.R Alis sinyali geldi.")
            order_side = 'BUY'
            return order_side

    if wr_value > float(globals.wr_max_value):
        bb_control = signal_control_by_bb(coin_data)

        last_signal_control = check_last_signal(i, processing_time, signal_type)

        if bb_control and last_signal_control:

            print("W.R Satis sinyali geldi.")
            order_side = 'SELL'
            return order_side

    return False


def signal_control_by_bb(coin_data):

    if globals.bollinger_aspect == '1':

        bollinger_band_upper = coin_data.indicators['BB.upper']
        bollinger_band_lower = coin_data.indicators['BB.lower']
        close_value = coin_data.indicators['close']

        if bollinger_band_lower <= close_value <= bollinger_band_upper:
            return True
        else:
            return False

    else:
        return True


def check_last_signal(i, processing_time, signal_type):

    coin_code = globals.coin_list[i][0]

    processing_type = None

    if processing_time == '15m':
        processing_type = 15

    elif processing_time == '1h':
        processing_type = 60

    elif processing_time == '4h':
        processing_type = 240

    elif processing_time == '1d':
        processing_type = 1.440

    if globals.check_last_signal_value == '1' and processing_type:

        result = select.last_proccess_control(coin_code, processing_time, signal_type, processing_type)

        if result:
            return False

        else:
            return True

    else:
        return True


def high_popularity_indicator(processing_time):
    # Binance Futures API uç noktası
    url_ticker = "https://fapi.binance.com/fapi/v1/ticker/24hr"
    url_exchange_info = "https://fapi.binance.com/fapi/v1/exchangeInfo"

    # 24 saatlik değişim verilerini al
    response_ticker = requests.get(url_ticker)
    data_ticker = response_ticker.json()

    # Borsa bilgi verilerini al
    response_exchange_info = requests.get(url_exchange_info)
    data_exchange_info = response_exchange_info.json()

    # %10'dan büyük değişim oranlarını ve mevcut değerlerini filtrele
    filtered_data = []
    for ticker in data_ticker:
        price_change_percent = float(ticker['priceChangePercent'])
        if price_change_percent > 10:  # %10'dan büyük değişimleri filtrele
            filtered_data.append({
                'symbol': ticker['symbol'],
                'price_change_percent': price_change_percent,
                'last_price': ticker['lastPrice']
            })

    # Sembol bazında min_unit ve price_unit bilgilerini almak için exchangeInfo verilerini kullan
    symbol_info = {item['symbol']: item for item in data_exchange_info['symbols']}

    for index in filtered_data:
        coin_code = index['symbol']

        # Sembolün trade bilgilerini al
        info = symbol_info.get(coin_code, {})

        # Min birim ve fiyat adımı bilgilerini al
        min_unit = None
        price_unit = None

        for filter in info.get('filters', []):
            if filter['filterType'] == 'LOT_SIZE':
                min_unit = filter['minQty']
            elif filter['filterType'] == 'PRICE_FILTER':
                price_unit = filter['tickSize']

        coin_code = coin_code + '.P'
        try:
            # Teknik analiz ve sinyal işlemleri
            coin = TA_Handler(
                symbol=coin_code,
                screener="crypto",
                exchange="BINANCE",
                interval=processing_time,  # İşlem aralığı
            )

            coin_data = coin.get_analysis()
            wr_value = coin_data.indicators['W.R']
            signal_type = 'HPI'
            order_type = 'Market'

            if wr_value < -90:
                print(f"{coin_code} için yüksek popülarite göstergesi alım sinyali geldi.")
                order_side = 'BUY'
                # İşlem açma fonksiyonunu çağır
                opened_function(min_unit, index['last_price'], price_unit, coin_data, coin_code, order_side, signal_type, order_type, "1d")

        except:
            pass


def opened_function(min_unit, close_value, price_unit, coin_data, coin_code, order_side, signal_type, order_type, processing_time):
    try:
        precision_qty = len(min_unit.split('.')[1])
        quantity = float(globals.order_price) / close_value
        quantity = float(format(quantity, f'.{precision_qty}f'))

    except:
        quantity = float(globals.order_price) / close_value
        quantity = int(quantity)

    open_price = close_value

    precision_cls = len(price_unit.split('.')[1])
    close_price = float(coin_data.indicators['VWMA'])
    close_price = close_price - (close_price - open_price) * float(globals.distance_closing_value)
    close_price = float(format(close_price, f'.{precision_cls}f'))
    indicator_value = coin_data.indicators['W.R']

    stop_price = open_price + (open_price - close_price)

    call_back_rate = (((close_price - open_price) / open_price) * 100) / 2

    call_back_rate = abs(round(call_back_rate, 2))

    if call_back_rate > globals.max_call_back_rate:
        call_back_rate = globals.max_call_back_rate

    if quantity > 0 and call_back_rate > 0.2:
        print(coin_code, " de ", order_side, " tipinde ", signal_type, " sinyalli ", "call back rate = ", call_back_rate)
        insert.open_order_signal(coin_code, order_side, order_type, processing_time, quantity, open_price, close_price, signal_type, indicator_value, stop_price, call_back_rate)
