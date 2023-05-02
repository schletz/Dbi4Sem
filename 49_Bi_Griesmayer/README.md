# Umsetzung der BI Themen von griesmayer.com mit Python

Voraussetzungen: Python und Jupyter Umgebung, siehe [Kapitel Jupyter Notebooks](../50_JupyterNotebooks/README.md)
Einige Übungen laden Exceldateien im alten Excel Format (xls).
Daher muss vorher das Python Paket *xlrd* in der Konsole installiert werden:

```
pip3 install xlrd --upgrade
```

## ETL_MLoad

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=01_ETL_MLoad  
**Jupyter Notebook:** [sales.ipynb](01_etl_mload/sales.ipynb)

Hinweis: Du kannst für das Erstellen eigener Datenfiles in der Übung das Tool [mockaroo](https://www.mockaroo.com) verwenden.

## ETL_RecordStartDate

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=02_ETL_RecordStartDate  

**Jupyter Notebooks:**
- **[record_start_date.ipynb](02_etl_recordstartdate/record_start_date.ipynb):**
  Dieses Notebook lädt die *Customer_20170201* bis *Customer_20170204* Dateien als Alternative zu den Integration Services.
- **[employee.ipynb](02_etl_recordstartdate/employee.ipynb):**
  Für die Übungsaufgabe. Dieses Notebook generiert die Dateien *employee_20170225* bis *employee_20170228* mit zufälligen Werten und führt den Import durch.

## Cube

**Quelle:** http://griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=03_Cube

**SQL Dumps:**

Für die Übungen gibt es fertige SQL Dumps.
Sie ersparen das Laden der Excel Datei.
**[Download cube_sql_dumps.zip](03_cube/cube_sql_dumps.zip)**

**Jupyter Notebooks:**

Wenn du eine Python/Jupyter Umgebung hast, kannst du statt dessen die Rohdateien auch direkt mit Python in die Datenbank laden.

- [airport.ipynb](03_cube/airport.ipynb)
- [bar.ipynb](03_cube/bar.ipynb)
- [catering.ipynb](03_cube/catering.ipynb)
- [cinema.ipynb](03_cube/cinema.ipynb)
- [note.ipynb](03_cube/note.ipynb)
- [online_order.ipynb](03_cube/online_order.ipynb)
- [share.ipynb](03_cube/share.ipynb)
- [university.ipynb](03_cube/university.ipynb)

## Star Schema

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=04_StarSchema  
**SQL Dump zum Weboster Modell:** [webhoster.sql](04_star_schema/webhoster.sql) (für SQL Server)
