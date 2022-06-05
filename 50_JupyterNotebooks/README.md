# Datenauswertung mit Python: Jupyter Notebooks

## Voraussetzungen

Um ein Jupyter Notebook zu erstellen, muss folgendes vorbereitet werden:

- Installation der neuesten Python Version von https://www.python.org/downloads/.
  Bitte *Custom Installation* wählen und als Pfad *C:\Python3* wählen.
  Bitte auch *Add to PATH* (Umgebungsvariable registrieren) auswählen.
  Geben Sie zur Kontrolle den Befehl *python --version* ein. Er muss die Version ausgeben. Falls
  *Befehl nicht gefunden* erscheint, ist die PATH Variable nicht korrekt gesetzt.
- Installation von Visual Studio Code (wenn noch nicht vorhanden)
  - Installation der Extension [Jupyter](https://marketplace.visualstudio.com/items?itemName=ms-toolsai.jupyter) in VS Code.
  - Installation der Extension [Python](https://marketplace.visualstudio.com/items?itemName=ms-python.python) in VS Code.
- Aktualisieren des Packaga Managers und Installieren der folgenden Pakete in der Konsole:

```
python -m pip install --upgrade pip
pip install ipykernel --upgrade
pip install sqlalchemy --upgrade
pip install pyodbc --upgrade
pip install requests --upgrade
pip install numpy --upgrade
pip install matplotlib --upgrade
pip install pandas --upgrade
pip install scipy --upgrade
pip install statsmodels --upgrade
pip install nbconvert[webpdf] --upgrade
```

Mit dem Parameter *upgrade* wird die aktuellste Version installiert, falls vorher durch
dependencies eine andere Versions schon installiert wurde. Durch das letzte Paket können Jupyter
Notebooks in ein PDF konvertiert werden.

### Vorhandene SQL Server Datenbank

Das erste Beispiel verwendet die generierte Fahrkarten Datenbank in SQL Server. Dafür muss
eine SQL Server Installation vorhanden sein. Entweder sie wird wie im vorigen Schritt
([41_SqlServerInstall](../41_SqlServerInstall/README.md)) installiert oder es wird ein Docker
Image von SQL Server verwendet.

#### SQL Server als Docker Image

Wenn [Docker für Windows](https://docs.docker.com/desktop/windows/install/#install-docker-desktop-on-windows)
vorhanden ist, kann SQL Server 2019 auch als Container laufen.
Der folgende Befehl startet einen SQL Server 2019 Container und weist das Passwort *SqlServer2019*
zu. Es muss beim Datengenerator angegeben werden.

```
docker run -d -p 1433:1433  --name sqlserver2019 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" mcr.microsoft.com/mssql/server:2019-latest      
```

### Musterdaten Generator

In [42_DatabaseGenerator](../42_DatabaseGenerator/README.md) gibt es ein .NET Programm, welches
Musterdaten in die SQL Server Datenbank schreibt.

## Das erste Skript: Verbinden zur Datenbank

Auf [docs.microsoft.com](https://docs.microsoft.com/en-us/sql/connect/python/pyodbc/step-3-proof-of-concept-connecting-to-sql-using-pyodbc?view=sql-server-ver15)
ist ein kleines Beispiel für ein Python Skript zur Verbindung mit der Datenbank. In der vorigen
Übung haben wir SQL Server mit dem Instanznamen *SQLSERVER2019* installiert. Der Admin User (*sa*)
hat das Kennwort *1234*. Mit diesen Daten versuchen wir nun, eine Verbindung mit der Datenbank
herzustellen.

Wird der Docker Container von SQL Server wie oben erwähnt verwendet, ist der Host *127.0.0.1*
und das Passwort *SqlServer2019*.

Dafür erstellen wir eine Datei mit dem Namen *dbAccess.py* in Visual Studio Code. Kopieren Sie danach
folgenden Inhalt in die leere Datei:

```python
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
```

Das Skript kann auf 2 Arten ausgeführt werden:

- Durch die Eingabe von *python dbAccess.py* in der Konsole.
- Durch Drücken von *F5* in Visual Studio Code. Hier startet der Debugger. Bei der Konfiguration muss
  beim ersten Start *Python File* ausgewählt werden. Es können auch - wie in Visual Studio - Breakpoints
  gesetzt werden. Mit *F10* kann das Programm Schritt für Schritt durchgegangen werden.

## Jupyter Notebooks

Im Data Science Bereich müssen oft dynamische Dokumente hergestellt werden. Das sind Dokumente
auf Markdown Basis, die aber Berechnungen beinhalten. Einige Programme wie z. B. Mathcad verfolgen
ähnliche Ideen.

Um ein Jupyter Notebook anzulegen, öffnen Sie mit *F1* (oder *STRG+SHIFT+P*) die Einstellungen von
VS Code. Dort finden Sie *Create: New Jupyter Notebook*. Speichern Sie das Notebook als
*firstNotebook.ipynb* ab.

Nun fügen wir eine Codezelle mit dem kleinen Skript von oben ein und starten es. Wir sehen nun
die Ausgaben im Notebook. 

### Ausgabe als PDF

Die Ausgaben des Jupyter Notebooks können wir auch als PDF weitergeben. Der Export geschieht
in der Kommandozeile:

```
jupyter nbconvert --to WebPDF --TemplateExporter.exclude_input=True --allow-chromium-download (file.ipynb)
```

Um Zeilenumbrüche beim Export zu vermeiden, kann vor der Hauptüberschrift eine kleine style Anweisung
eingegeben werden:
```md
<style>
    .jp-Cell {
        break-inside: avoid;
    }
    .jp-mod-noOutputs {
        display:none;
    }
</style>
# Meine Hauptüberschrift
```

### Data Wrangling

Unter Data Wrangling versteht man das Aufbereiten von Daten für die weitere Verarbeitung. Dafür
gibt es im Paket *pandas* eine eigene Datenstruktur: den *Dataframe*.

In der Datei [firstNotebook.ipynb](firstNotebook.ipynb) hier im Repository sind einige Beispiele
eingefügt.

**[Zum Cheat Sheet](https://pandas.pydata.org/Pandas_Cheat_Sheet.pdf)**

## Übung

Erstellen Sie ein Jupyter Notebook mit dem Namen *salesAnalysis.ipynb*. Fügen Sie in die erste
Zelle den Code ein, der die Tabelle *Verkauf* aus der SQL Server Datenbank in einen Dataframe
mit dem Namen *sales* einliest. Erstellen Sie danach eine Zelle für jede Aufgabe.

- Geben Sie die Verkäufe zwischen 1.12.2019 und 7.12.2019 0:00 aus. Sie können das Datum als
  String im dem Format *2019-12-01* angeben.
- Ordnen Sie die Verkäufe nach dem Datum und geben Sie die ersten 10 Verkäufe aus.
- Was ist der höchste Wert der Menge eines Verkaufes? Verwenden Sie *max()*.

Für die nachfolgenden Beispiele interessiert uns auch die Kartenart. Erstellen Sie dafür einen
Dataframe *salesWithCardtype*. Befüllen Sie den Dataframe mit einem SQL Statement, welches
einen JOIN zwischen Verkäufe und die Kartenart herstellt.

- Was ist der höchste Wert der Menge eines Verkaufes der Kartenart "Wochenkarte"? Verwenden Sie *max()*.
- Listen Sie alle Verkäufe mit einem Umsatz über 200 Euro auf. Der Umsatz pro Verkauf ist
  Menge * Preis der Kartenart. Definieren Sie vorher eine neue Spalte *Umsatz* im Dataframe.
- Berechnen Sie den Umsatz pro Kartenart. Informieren Sie sich auf der Seite
  https://pandas.pydata.org/pandas-docs/stable/reference/api/pandas.DataFrame.groupby.html?highlight=groupby#pandas.DataFrame.groupby
  über die groupby Methode des Dataframes und wie sie die Spalte *Umsatz* pro Kartenart
  summieren können.
