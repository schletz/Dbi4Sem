# Generierung der Musterdatenbank

## Programm zum Generieren der Musterdatenbank

Starten Sie das .NET Core Programm im Ordner DataGenerator mit dem Befehl

```text
dotnet run -c Release
```

Das Programm erstellt die SQL Server Datenbank und fügt 10000 Verkäufe ein. Für eine genauere
Analyse ist aber ein Datenbestand von 100 000 Verkäufen nötig. Das Programm schreibt diese Daten
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
