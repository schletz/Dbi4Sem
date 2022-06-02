import pyodbc
import sqlalchemy

connection_url = sqlalchemy.engine.URL.create(
    "mssql+pyodbc",
    username="sa",
    password="1234",               # oder SqlServer2019 (Docker Image)
    host=".\SQLSERVER2019",        # oder 127.0.0.1 (Docker Image)
    database="Fahrkarten",
    query={
        "driver": "ODBC Driver 17 for SQL Server"
    },
)

engine = sqlalchemy.create_engine(connection_url)
with engine.connect() as conn:
    result = conn.execute("SELECT * FROM Station")
    records = result.fetchall()
    for row in records:
        print(row[0], row[1], row[2])
