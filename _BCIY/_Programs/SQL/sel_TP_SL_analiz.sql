

--select sum(TP_SL)/count(*) from (
select SYMBOL, INTERVAL_TYPE, SIGNAL_TYPE, ORDER_SIDE, 100*sum(chg)/count(*) 'TP_SL' from (

--  select top 9 * from PROCESS
select SYMBOL, INTERVAL_TYPE, SIGNAL_TYPE, ORDER_SIDE
, OPENED_PRICE, CLOSED_PRICE
                                                     , case when ORDER_SIDE = 'BUY'  then case when CLOSED_PRICE >= OPENED_PRICE then (CLOSED_PRICE - OPENED_PRICE) / OPENED_PRICE
                                                                                                                                 else -1*(OPENED_PRICE - CLOSED_PRICE) / OPENED_PRICE
																			              end
                                                            when ORDER_SIDE = 'SELL' then case when CLOSED_PRICE >= OPENED_PRICE then -1*(CLOSED_PRICE - OPENED_PRICE) / OPENED_PRICE
                                                                                                                                 else (OPENED_PRICE - CLOSED_PRICE) / OPENED_PRICE
																			              end
												       end chg
from PROCESS
where 1=1
--     and  SYMBOL = 'ATOMUSDT.P' and INTERVAL_TYPE = '15m' and SIGNAL_TYPE = 'RSI' --and ORDER_SIDE = 'BUY'

and CLOSED_DATETIME >= '2024-05-01 00:00:00.000'
and status = 'CLOSED' ) xx
--order by ORDER_SIDE
group by SYMBOL, INTERVAL_TYPE, SIGNAL_TYPE, ORDER_SIDE
order by SYMBOL, INTERVAL_TYPE, SIGNAL_TYPE, ORDER_SIDE
--) xx
--having count(*) > 1
--select '2024-05-04 22:11:41.493'

/*
SYMBOL	INTERVAL_TYPE	SIGNAL_TYPE	ORDER_SIDE	OPENED_PRICE	CLOSED_PRICE	(No column name)
ATOMUSDT.P	15m	RSI	BUY	9.04100000	9.03300000	-0.00088485786970467
ATOMUSDT.P	15m	RSI	BUY	8.97800000	9.03300000	0.00612608598797059
ATOMUSDT.P	15m	RSI	BUY	8.93500000	9.03300000	0.01096810296586458
ATOMUSDT.P	15m	RSI	SELL	9.17500000	9.12800000	0.00512261580381471
ATOMUSDT.P	15m	RSI	SELL	9.29100000	9.12800000	0.01754385964912281
ATOMUSDT.P	15m	RSI	SELL	9.32700000	9.29600000	0.00332368392837997


ID	STATUS	SYMBOL	ORDER_SIDE	ORDER_TYPE	INTERVAL_TYPE	QUANTITY	OPEN_PRICE	CLOSE_PRICE	CLOSE_TYPE	OPENED_PRICE	EXECUTED_QUANTITY	CLOSED_PRICE	SIGNAL_TYPE	BINANCE_ORDER_ID	BINANCE_CLOSE_ORDER_ID	BINANCE_STATUS	INSERT_DATETIME	UPDATE_DATETIME	CLOSED_DATETIME
1588	CLOSED	LINKUSDT.P	BUY	MARKET	15m	3.48000000	14.37700000	14.34900000	NULL	14.37600000	3.48000000	14.34900000	W.R	32361604080	32361606107	FILLED	2024-05-05 21:15:24.000	2024-05-06 02:01:10.413	2024-05-06 02:10:40.097
1590	CLOSED	NEOUSDT.P	BUY	MARKET	15m	3.00000000	16.65600000	16.63300000	NULL	16.66011000	3.00000000	16.66700000	W.R	8298275076	8298276160	FILLED	2024-05-05 21:15:24.000	2024-05-06 02:01:11.303	2024-05-06 02:04:54.270
*/



