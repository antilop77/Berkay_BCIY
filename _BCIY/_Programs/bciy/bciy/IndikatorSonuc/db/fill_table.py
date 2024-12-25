import pyodbc
import time

from config import CONFIG

driver = CONFIG['db']['DRIVER']
server = CONFIG['db']['SERVER']
database = CONFIG['db']['NAME']
db_id = CONFIG['db']['USERNAME']
password = CONFIG['db']['PASSWORD']

conn = pyodbc.connect('DRIVER=' + driver + '; \
                           SERVER=' + server + '; \
                           DATABASE=' + database + ';\
                           UID=' + db_id + ';\
                           PWD=' + password + ';\
                           ')
cursor = conn.cursor()


def indikator_sonuc_update(indikator_veri_id, indikator_sonucu):
    cursor.execute("UPDATE IndikatorVerileri SET Sonuc='%s' WHERE ID='%s'" % (indikator_sonucu, indikator_veri_id))
    conn.commit()
