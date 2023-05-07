# Umsetzung der BI Themen von griesmayer.com mit Python

Voraussetzungen: Python und Jupyter Umgebung, siehe [Kapitel Jupyter Notebooks](../50_JupyterNotebooks/README.md)
Einige Übungen laden Exceldateien im alten Excel Format (xls).
Daher muss vorher das Python Paket *xlrd* in der Konsole installiert werden:

```
pip3 install xlrd --upgrade
```

## ETL_MLoad

Diese Lektion lädt Textdateien, die täglich geschrieben werden, in eine Datenbank.
Die Dateien überschneiden sich nicht, d. h. es wird jede Textdatei vollständig in die Tabelle geladen.
Die Textdateien haben leere Zeilen und Fußzeilen, die nicht geladen werden sollen.

> Im Video wird das Laden mit den SQL Server Integration Services gezeigt.
> Die Integration Services laufen nicht im Docker Container.
> Du brauchst eine SQL Server Instanz auf einem Windows Rechner, der diese Services anbietet.

Die hier angebotenen Materialien zeigen das Laden über Jupyter Notebooks in Python, hierfür genügt ein SQL Server Docker Container.

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=01_ETL_MLoad  
**Jupyter Notebook:** [sales.ipynb](01_etl_mload/sales.ipynb)

Hinweis: Du kannst für das Erstellen eigener Datenfiles in der Übung das Tool [mockaroo](https://www.mockaroo.com) verwenden.

## ETL_RecordStartDate

Diese Lektion lädt Textdateien, die täglich geschrieben werden, in eine Datenbank.
Diese Dateien beinhalten allerdings Stammdaten, dessen Änderung berücksichtigt werden soll.
Ändern sich die Daten nicht, wird der Datensatz auch nicht erneut in die Tabelle geladen.
Ändert sich der Datensatz, oder ist er nicht mehr vorhanden, wird ein *RecordFrom* und *RecordTo* Feld geschrieben, das angibt, in welchem Zeitraum der Datensatz gültig ist.

> Im Video wird das Laden mit den SQL Server Integration Services gezeigt.
> Die Integration Services laufen nicht im Docker Container.
> Du brauchst eine SQL Server Instanz auf einem Windows Rechner, der diese Services anbietet.

Die hier angebotenen Materialien zeigen das Laden über Jupyter Notebooks in Python, hierfür genügt ein SQL Server Docker Container.

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=02_ETL_RecordStartDate  

**Jupyter Notebooks:**
- **[record_start_date.ipynb](02_etl_recordstartdate/record_start_date.ipynb):**
  Dieses Notebook lädt die *Customer_20170201* bis *Customer_20170204* Dateien als Alternative zu den Integration Services.
- **[employee.ipynb](02_etl_recordstartdate/employee.ipynb):**
  Für die Übungsaufgabe. Dieses Notebook generiert die Dateien *employee_20170225* bis *employee_20170228* mit zufälligen Werten und führt den Import durch.

## Cube

Diese Lektion erstellt, ausgehend von einem Star Schema, einen CUBE mittels SQL Server Analysis Services.
Die hier angebotenen Materialien befüllen die Datenbasis, nämlich die SQL Server Datenbank.
Du kannst entweder den SQL Dump direkt einspielen oder das Jupyter Notebook verwenden.

> Die Analysis Services laufen nicht im Docker Container.
> Du brauchst eine SQL Server Instanz auf einem Windows Rechner, der diese Services anbietet.

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

Für die Übung gehe am Besten so vor:

- Erzeuge mit [mockaroo](https://www.mockaroo.com) zuerst die Dimension Tables. Wähle als primary key die row number.
- Erzeuge danach die Fact Table. Für den Fremdschlüssel zur Dimension Table wähle in mockaroo den Typ *number* und weise Werte von 1 bis n (n = Anzahl der Werte der Dimension Table) zu, sodass der Fremdschlüssel auf einen gültigen Wert in der Dimension Table zeigt.
- Beachte, dass mockaroo maximal 1000 Werte im kostenlosen Modus exportieren kann.
  Wenn du mehr Daten brauchst, muss auf einen Faker in Python, .NET, etc. zurückgegriffen werden.

## PowerBI

Diese Lektion lädt Sales Daten in PowerBI und visualisiert sie mit Diagrammen.
Zur Vereinfachung gibt es den fertigen SQL Dump, der in den SQL Server Container eingespielt werden kann.
Du kannst dann mit PowerBI direkt auf die SQL Server Datenbank zugreifen.

**Quelle:** http://www.griesmayer.com/?menu=Business%20Intelligence&semester=Semsester_6&topic=05_PowerBI  
**Jupyter Notebook:** [sales.ipynb](05_powerbi/sales.ipynb)  
**SQL Dump:** [sales_sql.zip](05_powerbi/sales_sql.zip)