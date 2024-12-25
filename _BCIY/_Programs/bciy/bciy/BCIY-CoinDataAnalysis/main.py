import datetime
import sys
import threading
import config
import time
from db import select, update

from script import coin_analysis

try:
    indicator = str(sys.argv[1])
except:
    indicator = "W.R"

print(datetime.datetime.now(), " <--", indicator, "<-- Icin Islem Basladi.")
working_threads = 0
working_threads_lock = threading.Lock()
first_time = datetime.datetime.now()
datetime1 = datetime.datetime.now()
time_unit_list = config.CONFIG['parameter']['PROCESSING_TIME_LIST']


def main(thread_num, coin_code, time_unit):
    coin_analysis.coin_analysis(thread_num, coin_code, time_unit, indicator)
    global working_threads
    with working_threads_lock:
        working_threads = working_threads - 1


def run_threads():
    try:
        global working_threads
        global first_time
        thread_count = config.CONFIG['parameter']['THREADS_COUNT']
        coins = select.withdraw_coins()

        global datetime1
        datetime1 = datetime.datetime.now()
        if coins is None:
            print(datetime1, "indicator: ", indicator)
            time.sleep(30)
            return

        threads = []

        for x in range(len(coins)):
            for time_unit in time_unit_list:
                try:
                    for i in range(len(coins)):
                        while True:
                            if working_threads < thread_count:
                                with working_threads_lock:
                                    working_threads = working_threads + 1
                                thread = threading.Thread(target=main, args=(i + 1, coins[i][0], time_unit))
                                threads.append(thread)
                                thread.start()
                                break

                    for thread in threads:
                        thread.join()
                except Exception as e:
                    print(e, "len(coins))")
    except Exception as e:
        print(e, "run_threads")


while True:
    try:
        datetime2 = datetime.datetime.now()

        run_threads()
        time_difference = datetime2 - datetime1
        total_seconds = time_difference.total_seconds()
        print(datetime2, " bitti :)", " indicator = ", indicator)
        print(total_seconds, " <--- Toplam Islem Suresi", indicator, " <-- İslem Tipi")
        time.sleep(3600)

    except Exception as e:
        print(e, "while True")
