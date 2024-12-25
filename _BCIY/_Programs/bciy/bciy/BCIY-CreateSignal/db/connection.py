import pyodbc
from config import CONFIG


def cursor_function():
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
    return cursor
