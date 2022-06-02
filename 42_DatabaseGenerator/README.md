# Generierung der Musterdatenbank, Zeitreihenanalyse

## Programm zum Generieren der Musterdatenbank

Starten Sie das .NET 6 Programm im Ordner DataGenerator mit dem Befehl
*dotnet run -c Release* im Ordner *42_DatabaseGenerator/DataGenerator*:

```text
Dbi4Sem\42_DatabaseGenerator\DataGenerator>dotnet run -c Release
```

Danach werden Serveradresse, Datenbankname, ... abgefragt. In den eckigen Klammern sind die
Standardwerte angegeben, die mit der vorigen SQL Server Installation funktionieren.

Das Programm erstellt die SQL Server Datenbank und fügt 10000 Verkäufe ein. Für eine genauere
Analyse ist aber ein Datenbestand von 1 000 000 Verkäufen nötig. Das Programm schreibt diese Daten
in eine Tab getrennte Textdatei.

```text
DATENGENERATOR FÜR VERKÄUFE
Erstelle die Datenbank...
Füge die Daten ein (10000 Verkäufe)...
10000 Verkäufe in 3.0 s eingefügt.
100000 Verkäufe in die Datei C:\Temp\verkauf.tsv geschrieben.

TRUNCATE TABLE Verkauf;
BULK INSERT Verkauf
FROM 'C:\Temp\verkauf.tsv' WITH (
        CODEPAGE  = '65001',
        FIRSTROW = 1,
        FIELDTERMINATOR = '\t'
);
```

## Durchführen einer einfachen Auswertung

Nach dem Erstellen der Datenbank muss noch die Timetable Tabelle erstellt und der Inhalt
geladen werden. Kopieren Sie vorher die Datei [Timetable.tsv](Timetable.tsv) in das
Verzeichnis *C:\Temp*. Führen Sie dann den folgenden SQL Dump aus:

```sql
CREATE TABLE Timetable (    
    Datum        DATE PRIMARY KEY,
    Datum2000    DATE NOT NULL,
    Jahr         INTEGER NOT NULL,
    Monat        INTEGER NOT NULL,
    Tag          INTEGER NOT NULL,
    WochentagNr  INTEGER NOT NULL,
    WochentagStr CHAR(2) NOT NULL,
    Feiertag     VARCHAR(32) NOT NULL,
    WerktagMoFr  INTEGER NOT NULL,
    WerktagMoSa  INTEGER NOT NULL
);    
GO

BULK INSERT Timetable    
FROM 'C:\Temp\Timetable.tsv' WITH (    
    CODEPAGE  = '65001',
    FIRSTROW = 2,
    FIELDTERMINATOR = '\t'
);    
GO

TRUNCATE TABLE Verkauf;
BULK INSERT Verkauf
FROM 'C:\Temp\verkauf.tsv' WITH (
        CODEPAGE  = '65001',
        FIRSTROW = 1,
        FIELDTERMINATOR = '\t'
);
GO

CREATE VIEW vUmsatzstatistik AS
SELECT 
t.Datum,
t.WochentagStr,
v.VerkaufId, 
k.KartenartId,
DATEPART(hour, v.Datum) AS Stunde,
v.Menge, 
s.StationId,
v.Menge * k.Preis AS Umsatz
FROM Timetable t LEFT JOIN Verkauf v ON (t.Datum = CAST(v.Datum AS DATE))
                 LEFT JOIN Kartenart k ON (v.KartenartId = k.KartenartId)
                 LEFT JOIN Station s ON (v.StationId = s.StationId)
WHERE t.Datum >= (SELECT MIN(v2.Datum) FROM Verkauf v2) AND 
      t.Datum <= (SELECT MAX(v2.Datum) FROM Verkauf v2);
GO


CREATE VIEW vStundenumsatz AS
SELECT Datum, WochentagStr, Stunde, SUM(Umsatz) AS Umsatz
FROM vUmsatzstatistik
GROUP BY Datum, WochentagStr, Stunde;
GO

CREATE VIEW vUmsatzanalyse AS
SELECT Datum AS Tag, WochentagStr, Stunde, 
DATEADD(hour, Stunde, CAST(Datum AS DATETIME)) AS Datum,
Umsatz,
AVG(Umsatz) OVER(PARTITION BY WochentagStr, Stunde) AS Stundenmittel,
Umsatz - AVG(Umsatz) OVER(PARTITION BY WochentagStr, Stunde) AS Bias
FROM vStundenumsatz;
GO

SELECT *
FROM vUmsatzanalyse
WHERE Tag < '2019-03-01'
ORDER BY Datum
```

## Verfügbare Videos

### Installation von SQL Server

Dieses Video beschreibt die Installation von SQL Server 2019 Developer Edition mit den Analysis
Services auf einem Windows Rechner.

Verwendete Links:

- [SQL Server Downloadseite](https://www.microsoft.com/de-de/sql-server/sql-server-downloads)

Videolink: https://youtu.be/gqALFWGl0Bk

## Das SQL Server Management Studio und der DataGenerator

Nach der Installation von SQL Server muss das Management Studio für den Zugriff auf den Server
installiert werden. Außerdem werden Musterdaten mittels eines .NET Programmes erzeugt und in die
Datenbank *Fahrkarten* geschrieben.

Zum Anzeigen Ihres aktuellen Windowsbenutzers können Sie in der Konsole den Befehl *echo %username%*
absetzen. Falls Sie Windows mit einem Microsoft Benutzer installiert haben, kann der lokale
Benutzername nämlich abweichen.

Verwendete Links:

- [Download SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)

Videolink: https://youtu.be/ZogvBKhvLko

## Warum Analysis Services?

Dieses Video erklärt die Auswertung von Daten ohne Analysis Services. Am Beispiel von Excel werden
die Daten in eine Pivot Tabelle übertragen und dargestellt. Der Nachteil: Die Datenbank wird
jedes mal abgefragt und das gesamte Abfrageergebnis wird an den Client übertragen.

Videolink: https://youtu.be/4loI2-U7VwY

## Übung

**(1)** Laden Sie von 
https://info.gesundheitsministerium.gv.at/data/timeline-faelle-bundeslaender.csv
die neuesten COVID Daten und laden Sie diese Datei in *C:\Temp*

**(2)**  Erstellen Sie eine SQL Server Datenbank COVID

```sql
CREATE DATABASE Covid
```

**(3)**  Erstellen Sie eine Tabelle für die Rohdaten. Achten Sie darauf, dass Sie sich
in der Datenbank *Covid* befinden. Sie können mit *USE Covid* die Datenbank wechseln.

```sql
CREATE TABLE CovidTimeline (
	Datum           DATETIME NOT NULL,
	BundeslandID    INTEGER,
	Name            VARCHAR(255),
	BestaetigteFaelleBundeslaender   INTEGER,
	Todesfaelle          INTEGER,
	Genesen              INTEGER,
	Hospitalisierung     INTEGER,
	Intensivstation      INTEGER,
	Testungen            INTEGER,
	TestungenPCR         INTEGER,
	TestungenAntigen	 INTEGER,
	PRIMARY KEY (Datum, BundeslandId)
);
```

**(4)**  Laden Sie die Textdatei in die Tabelle *CovidTimeline*.

```sql
BULK INSERT CovidTimeline    
FROM 'C:\Temp\timeline-faelle-bundeslaender.csv' WITH (    
    CODEPAGE  = '65001',
    FIRSTROW = 2,
    FIELDTERMINATOR = ';'
);    
```

**(5)**  Erstellen Sie eine View *vCovidAnalyse*, die die durchschnittlichen Hospitalisierungen
  pro BundeslandId und Wochentag auswertet. Sie können den Wochentag mit *DATEPART*
  aus dem Datum extrahieren.
  Die View soll folgende Spalten beinhalten:
  - Datum (nur die Tageskomponente, verwenden Sie daher *CAST*)
  - BundeslandId
  - Wochentag
  - Durchschn. Hospitalisierung für diesen Wochentag und die BundeslandId (Spaltenname AvgHospitalisierung)
  - Differenz Hospitalisierung für diesen Tag - obiger Durchschnittswert (Spaltenname Residual)
  
Diese View muss genausoviele Zeilen wie die originale Tabelle *CovidTimeline* haben. Da die Daten
lückenlos sind, benötigen Sie die Tabelle *Timetable* nicht.

**(6)**  Laden Sie diese Daten in Microsoft Excel und zeigen Sie die Spalten 
*AvgHospitalisierung* und *Residual* des Bundeslandes 10 (Österreich gesamt) als Diagramm an.
Was können Sie beobachten? Ist die Spalte
Residual ein zufälliges Rauschen oder zeigt sich eine Form? Können Sie den Schätzwert
"Mittelwert pro Bundesland und Wochentag" als Prognosewert verwenden?

**(Für Profis)** Unsere Zerlegung basierte auf dem Modell

```
y(t) = a + Residual
```

wobei a der Mittelwert der Hospitalisierung dieses Bundeslandes an einem Wochentag ist. Das Ergebnis
war allerdings nicht zufriedenstellend. Daher werden wir eine weitere Variable hinzufügen:

```
y(t) = a + b + Residual
```

b soll nun der *Trend*, also die durchschnittliche Hospitalisierung diese Bundeslandes der letzten 7 Tage sein
(7 Tages Mittelwert). Um das zu realisieren, müssen Sie zuerst eine Hilfsabfrage
*HospitalisierungAnalyze* erstellen. Da wir sie nur einmal brauchen, können wir sie mit *WITH*
definieren. Dadurch ist die erzeugte Abfrage nur im nachfolgenden Select verfügbar.

Die Abfrage *HospitalisierungAnalyze* berechnet den gleitenden Mittelwert. Die Differenz zum echten
Wert wird als Spalte *Residual* ausgegeben. Dadurch erreichen wir eine Zerlegung in *y = a + Residual*.

Für den zweiten Parameter mitteln wir die Spalte *Residual* nochmals nach Bundesland und Wochentag.
Die Differenz ist die neue Spalte *Residual*. Unter dem Text ist die Grundstruktur der Abfrage
angegeben.

Die Daten werden also in *y = AvgHospWeek + AvgHospWochentag + Residual* zerlegt. Zeigen Sie die
Daten in einem Exceldiagramm an.

```sql
WITH HospitalisierungAnalyze AS (
	SELECT
		-- ...
		AVG(Hospitalisierung) OVER(PARTITION BY BundeslandId ORDER BY Datum ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) AS AvgHospWeek,
		-- Differenz Hospitalisierung zu oberen Wert AS Residual
		FROM CovidTimeline
) 
SELECT 
- ...
AVG(Residual) OVER(PARTITION BY BundeslandId, Wochentag) AS AvgHospWochentag,
Residual - AVG(Residual) OVER(PARTITION BY BundeslandId, Wochentag) AS Residual
FROM HospitalisierungAnalyze
```

Ändern Sie den Zeitraum des Trends von 7 Tagen (*6 PRECEDING AND CURRENT ROW* in SQL) auf 
2, 14 oder 30 Tage. Was fällt Ihnen im Diagramm auf? Beobachten Sie, wie "glatt" die Trendlinie ist.

Eine Voraussage kann z. B. mit einer Polynominterpolation des Trends (*AvgHospWeek*) und Addieren
der Spalte *AvgHospWochentag* des entsprechenden Wochentages gemacht werden.

![](covid_diagram2.png)

Der Graph von *Residual* hat noch einen periodischen Anteil, da die Wochentagsperiode auch vom
Wert der aktuellen Hospitalisierung abhängt (wenn am Freitag z. B. 10% der Personen entlassen werden,
ergibt sich diese Abhängigkeit). Weiterführende Pakete in Python wie auf
https://machinelearningmastery.com/decompose-time-series-data-trend-seasonality
beschrieben berücksichtigen auch diesen Umstand.
