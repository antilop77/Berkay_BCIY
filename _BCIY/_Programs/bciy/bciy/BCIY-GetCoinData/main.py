import datetime
import sys
import threading
import config
import time
from db import select, insert
from script import get_coin_data_from_api

try:
    processing_time = str(sys.argv[1])
except:
    processing_time = "1m"

print(datetime.datetime.now(), " <--", processing_time, "<-- Icın Islem Basladi.")
working_threads = 0
working_threads_lock = threading.Lock()
first_time = datetime.datetime.now()
datetime1 = datetime.datetime.now()


def main(thread_num, file, coin_code):
    now_time = datetime.datetime.now()
    try:
        global working_threads
        print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} baslatildi. Zaman birimi =", processing_time, file=file)
        #print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} baslatildi. Zaman birimi =", processing_time)

        coin_data = get_coin_data_from_api.get_data(coin_code, processing_time)

        print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veriler alindi. Zaman birimi =", processing_time, file=file)
        #print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veriler alindi. Zaman birimi =", processing_time)

        exception = insert.coin_data_insert(coin_data, thread_num)
        if exception:
            print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veri tabanina kayit edilirken hata alindi. Hata =", exception, " Zaman birimi =", processing_time, file=file)
            #print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veri tabanina kayit edilirken hata alindi. Hata =", exception, " Zaman birimi =", processing_time)
        else:
            print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veri tabanina basarli kayit edildi. Zaman birimi =", processing_time, file=file)
            #print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} veri tabanina basarli kayit edildi. Zaman birimi =", processing_time)

    except Exception as e:
        print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} hata alindi. Hata =", e, " Zaman birimi =", processing_time, file=file)
        print(now_time, " ", coin_code, " <--- icin ", f"thread {thread_num} hata alindi. Hata =", e, " Zaman birimi =", processing_time)

    with working_threads_lock:
        working_threads = working_threads - 1


def run_threads():
    try:
        global working_threads
        global first_time
        thread_count = config.CONFIG['parameter']['THREADS_COUNT']
        coins = select.withdraw_coins()

        if coins is None:
            time.sleep(30)
            return

        thread_state = []

        for x in range(len(coins)):
            thread_state.append(0)

        with open("log-" + processing_time + ".txt", "a") as file:
            threads = []
            try:
                while True:
                    now = datetime.datetime.now()

                    if processing_time == '1m':
                        if now.minute == first_time.minute:
                            time.sleep(1)
                        else:
                            first_time = now
                            time.sleep(20)
                            break

                    elif processing_time == '15m':
                        if now.minute % 15 == 0:
                            time.sleep(20)
                            break
                        else:
                            time.sleep(1)

                    elif processing_time == '1h':
                        if now.minute == 0:
                            time.sleep(20)
                            break
                        else:
                            time.sleep(1)

                    elif processing_time == '4h':
                        if (now.hour + 1) % 4 == 0:
                            time.sleep(20)
                            break
                        else:
                            time.sleep(1)

                    elif processing_time == '1d':
                        if now.hour == 3:
                            time.sleep(20)
                            break
                        else:
                            time.sleep(1)

            except Exception as e:
                print("Hata =", e, " Zaman birimi =", processing_time, file=file)
                print("Hata =", e, " Zaman birimi =", processing_time)

            time.sleep(1)

            try:
                global datetime1
                datetime1 = datetime.datetime.now()
                print(datetime1, " basladi :)", " işlem zaman birimi = ", processing_time)
                for i in range(len(coins)):
                    while True:
                        if working_threads < thread_count:
                            with working_threads_lock:
                                working_threads = working_threads + 1
                            thread = threading.Thread(target=main, args=(i + 1, file, coins[i][0]))
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
        run_threads()
        datetime2 = datetime.datetime.now()
        time_difference = datetime2 - datetime1
        total_seconds = time_difference.total_seconds()
        print(datetime2, " bitti :)", " işlem zaman birimi = ", processing_time)
        print(total_seconds, " <--- Toplam Islem Suresi", processing_time, " <-- İslem Tipi")

        if processing_time == '15m':
            time.sleep(61)
        elif processing_time == '1h':
            time.sleep(61)
        elif processing_time == '4h':
            time.sleep(3601)
        elif processing_time == '1d':
            time.sleep(3601)

    except Exception as e:
        print(e, "while True")
