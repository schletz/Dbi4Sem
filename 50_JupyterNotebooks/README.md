# Datenauswertung mit Python: Jupyter Notebooks

## Voraussetzungen

Um überhaupt mit Python arbeiten zu können, muss natürlich Python selbst und einige Pakete installiert werden.

### Windows

- Installation der neuesten Python Version von https://www.python.org/downloads/.
  Bitte *Custom Installation* wählen und als Pfad *C:\Python3* wählen.
  Danach *Add to PATH* (Umgebungsvariable registrieren) auswählen.
  Gib zur Kontrolle den Befehl *python --version* ein. Er muss die Version ausgeben. Falls
  *Befehl nicht gefunden* erscheint, ist die PATH Variable nicht korrekt gesetzt.
- Installiere danach den [Microsoft ODBC Driver 18 for SQL Server von der Microsoft Downloadseite.](https://learn.microsoft.com/en-us/sql/connect/odbc/download-odbc-driver-for-sql-server)

Nun können in der Konsole die Pakete, die wir brauchen werden, installiert werden:

```
python -m pip install --upgrade pip
pip3 install ipykernel --upgrade
pip3 install pyodbc --upgrade
pip3 install sqlalchemy --upgrade
pip3 install requests --upgrade
pip3 install numpy --upgrade
pip3 install matplotlib --upgrade
pip3 install pandas --upgrade
pip3 install scipy --upgrade
pip3 install statsmodels --upgrade
pip3 install faker --upgrade
pip3 install nbconvert[webpdf] --upgrade
```

### macOS

Für macOS kann der Packetmanager [https://brew.sh](Homebrew) zur Installation von Python verwendet werden.
Zusätzlich muss noch das Paket *unixodbc* installiert werden, um auf den SQL Server Container zugreifen zu können.
Danach wird der SQL Server ODBC Treiber wie [auf microsoft.com beschrieben](https://learn.microsoft.com/en-us/sql/connect/odbc/linux-mac/install-microsoft-odbc-driver-sql-server-macos?view=sql-server-ver16) installiert.

**Installation von Homebrew (wenn nicht schon vorhanden)**

````
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
````

**Installation der Pakete**

Achte darauf, dass die Schritte auch erfolgreich ausgeführt werden konnten.
Ein Weiterarbeiten bei einem fehlerhaften Installationsschritt macht keinen Sinn.

```
brew install python@3.11
brew install unixodbc
```

```
brew tap microsoft/mssql-release https://github.com/Microsoft/homebrew-mssql-release
brew update
HOMEBREW_ACCEPT_EULA=Y brew install msodbcsql18 mssql-tools18

sudo ln -s /usr/local/etc/odbcinst.ini /etc/odbcinst.ini
sudo ln -s /usr/local/etc/odbc.ini /etc/odbc.ini
```

```
python3.11 -m pip install --upgrade pip
pip3 install ipykernel --upgrade
pip3 install --no-cache-dir --no-binary :all: pyodbc --upgrade
pip3 install sqlalchemy --upgrade
pip3 install requests --upgrade
pip3 install numpy --upgrade
pip3 install matplotlib --upgrade
pip3 install pandas --upgrade
pip3 install scipy --upgrade
pip3 install statsmodels --upgrade
pip3 install faker --upgrade
pip3 install nbconvert[webpdf] --upgrade
```

Mit dem Parameter *upgrade* wird die aktuellste Version installiert, falls vorher durch
dependencies eine andere Versions schon installiert wurde. Durch das letzte Paket können Jupyter
Notebooks in ein PDF konvertiert werden.

### Extensions für VS Code (alle OS)

- Installation von Visual Studio Code (wenn noch nicht vorhanden)
  - Installation der Extension [Jupyter](https://marketplace.visualstudio.com/items?itemName=ms-toolsai.jupyter) in VS Code.
  - Installation der Extension [Python](https://marketplace.visualstudio.com/items?itemName=ms-python.python) in VS Code.


### Verbindungstest zu einer SQL Server Datenbank

Zum Testen der Verbindung muss natürlich ein SQL Server Container laufen.
Falls er nicht schon angelegt wurde, kann mit dem folgenden Befehl ein Container erstellt werden:

```
docker run -d -p 1433:1433  --name sqlserver2019 -e "ACCEPT_EULA=Y" -e "ACCEPT_EULA_ML=Y" -e "SA_PASSWORD=SqlServer2019" mcr.microsoft.com/azure-sql-edge      
```

#### Befüllen der Datenbank

In [42_DatabaseGenerator](../42_DatabaseGenerator/README.md) haben wir für die Fahrkartenverkäufe eine Datenbank *Fahrkarten* erstellt. Auf diese Datenbank werden wir zugreifen.

#### Das erste Skript: Verbinden zur Datenbank

Erstelle in VS Code eine Datei *connect_test.ipynb*.
Die Endung *ipynb* bedeutet *Jupyter Notebook*.
Kopiere in den Codeblock folgende Anweisungen und führe sie aus.
Es werden mit einer Abfrage alle Datenbanken, die im SQL Server Container vorhanden sind, ausgegeben.

```python
import pyodbc
import sqlalchemy

connection_url = sqlalchemy.engine.URL.create(
    "mssql+pyodbc",
    username="sa",
    password="SqlServer2019",
    host="localhost",
    database="tempdb",
    query={
        "driver": "ODBC Driver 18 for SQL Server",
    }
)
engine = sqlalchemy.create_engine(connection_url, connect_args={
        "TrustServerCertificate": "yes"
    })
with engine.connect() as conn:
    result = conn.execute(sqlalchemy.text("SELECT name, database_id, create_date FROM sys.databases"))
    records = result.fetchall()
    for row in records:
        print(row[0], row[1], row[2])

```

### Verbinden mit einer Oracle Datenbank

Natürlich kann man mit Python auch eine Oracle Datenbank ansprechen.
Ein Beispiel ist im Notebook [jupyter_oracle.ipynb](jupyter_oracle.ipynb).

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
