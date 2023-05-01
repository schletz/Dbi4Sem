# Umsetzung der BI Themen von griesmayer.com mit Python

Voraussetzungen: Python und Jupyter Umgebung, siehe [Kapitel Jupyter Notebooks](../50_JupyterNotebooks/README.md)
Einige Übungen laden Exceldateien im alten Excel Format (xls).
Daher muss vorher das Python Paket *xlrd* in der Konsole installiert werden:

```
pip3 install xlrd --upgrade
```

## ETL_MLoad

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=01_ETL_MLoad  
**Jupyter Notebooks:** [sales.ipynb](01_etl_mload/sales.ipynb)

## ETL_RecordStartDate

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=02_ETL_RecordStartDate  
**Jupyter Notebooks:**
- [record_start_date.ipynb](02_etl_recordstartdate/record_start_date.ipynb)
- [employee.ipynb](02_etl_recordstartdate/employee.ipynb) (Übungsaufgabe)

## Cube

**Quelle:** http://griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=03_Cube  
**Jupyter Notebooks:**
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
