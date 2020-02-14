# Inkrementelles Laden mit SQL*Loader

## Intro

Auf
[data.gv.at](https://www.data.gv.at/katalog/dataset/stadt-wien_wienerlinienechtzeitdaten)
stehen die Daten der Wiener Linien (Haltestellen, Linien, Steige) als
CSV Datei zur Verfügung. Die Dateien sind UTF-8 codiert, haben als
Trennzeichen ein Semikolon (;) und Strings sind unter Anführungszeichen
gesetzt. Zeilenumbruch ist der Windows Standard CR+LF. Sie können von
folgenden Adressen bezogen werden:

- *csv-linien*: https://data.wien.gv.at/csv/wienerlinien-ogd-linien.csv
- *csv-haltestellen*: https://data.wien.gv.at/csv/wienerlinien-ogd-haltestellen.csv
- *csv-steige*: https://data.wien.gv.at/csv/wienerlinien-ogd-steige.csv

### Die CSV Datei der Linien

Jede Linie hat in der Datei *wienerlinien-ogd-linien.csv* in der Spalte
*LINIEN_ID* eine ID, die als Fremdschlüssel in der Datei
*wienerlinien-ogd-steige.csv* in der Spalte *FK_LINIEN_ID* verwendet
wird. Daneben gibt es bei den Steigen noch die Spalte
*FK_HALTESTELLEN_ID*, die auf die entsprechende Haltestelle in der
Datei *wienerlinien-ogd-haltestellen.csv* verweist. Anbei die ersten 5
Zeilen der Liniendatei, nicht benötigte Spalten werden nicht angezeigt:

| LINIEN_ID  | BEZEICHNUNG | VERKEHRSMITTEL |
| ---------: | :---------- | :------------- |
|  215096838 | 99A         | ptBusCity      |
|  214433895 | 57A         | ptBusCity      |
|  214432881 | 10          | ptBusCity      |
|  218957255 | 79A         | ptBusCity      |
|  214433967 | N35         | ptBusNight     |

### Die CSV Datei der Steige

Wollen wir nun alle Haltestellen der Linie 12A (ID 214433815) wissen,
müssen wir die ID in der Datei Steige suchen und erfahren dort, dass
folgende Datensätze zugeordnet sind. Es wird bei der Reihenfolge
zwischen Hin (Richtung H) und Retour (Richtung R) unterschieden. Anbei
die ersten 5 Steige der Linie 12A, nicht benötigte Spalten werden nicht
angezeigt:

| STEIG_ID  | FK_LINIEN_ID   | FK_HALTESTELLEN_ID   | RICHTUNG | REIHENFOLGE | STEIG |
| --------: | -------------: | -------------------: | :------- | ----------: | :---- |
| 231475367 |      214433815 |            214461699 | H        |           1 | 48A-R |
| 231475368 |      214433815 |            214461699 | H        |           2 | 10A-H |
| 231475369 |      214433815 |            214460117 | H        |           3 | 10A-H |
| 231475370 |      214433815 |            214461744 | H        |           4 | 12A-H |
| 231475371 |      214433815 |            214460711 | H        |           5 | 12A-H |
| 231475428 |      214433815 |            214460372 | R        |           1 | 12A   |
| 231475429 |      214433815 |            214461309 | R        |           2 | 12A   |
| 231475430 |      214433815 |            214460695 | R        |           3 | 12A   |
| 231475431 |      214433815 |            214461152 | R        |           4 | 12A   |
| 231475432 |      214433815 |            214461310 | R        |           5 | 12A-R |

### Die CSV Datei der Haltestellen

Die Verknüpfung mit der Haltestellendatei gibt uns nun die Namen der
Haltestellen zurück. Folgendes Beispiel liefert die ersten 5
Haltestellen der Linie 12A in der Richtung “H” in der korrekten
Reihenfolge:

| HALTESTELLEN_ID  | NAME                    | REIHENFOLGE | STEIG |
| ---------------: | :---------------------- | ----------: | :---- |
|        214461699 | Schmelz, Gablenzgasse   |           1 | 48A-R |
|        214461699 | Schmelz, Gablenzgasse   |           2 | 10A-H |
|        214460117 | Auf der Schmelz         |           3 | 10A-H |
|        214461744 | Schanzstraße/Akkonplatz |           4 | 12A-H |
|        214460711 | Johnstraße              |           5 | 12A-H |

## 1. Schritt: Erstellen der Datenbank

Für eine Applikation sollen die Daten periodisch (z. B. 1x in der Nacht)
in eine lokale SQL Server oder Oracle Datenbank geladen werden. Diese
Datenbank besitzt den Namen *HaltestellenDb* (SQL Server) bzw. wird ein
User *HaltestellenDb* angelegt (Oracle). Die Datenbank besitzt folgendes
Schema (SQL Server):

``` sql
USE HaltestellenDb;
GO

DROP TABLE Steig;
DROP TABLE Haltestelle;
DROP TABLE Linie;

CREATE TABLE Linie (
    L_ID             INTEGER PRIMARY KEY,
    L_Bezeichnung    VARCHAR(200) NOT NULL,
    L_Verkehrsmittel VARCHAR(200) NOT NULL
);

CREATE TABLE Haltestelle (
    H_ID   INTEGER PRIMARY KEY,
    H_Name VARCHAR(200) NOT NULL
);

CREATE TABLE Steig (
    S_ID          INTEGER PRIMARY KEY,
    S_Linie       INTEGER NOT NULL,
    S_Haltestelle INTEGER NOT NULL,
    S_Steig       VARCHAR(10),
    S_Richtung    CHAR(1) NOT NULL,
    S_Reihenfolge INTEGER NOT NULL,
    FOREIGN KEY (S_Linie) REFERENCES Linie(L_ID),
    FOREIGN KEY (S_Haltestelle) REFERENCES Haltestelle(H_ID)
);
```

## 2. Schritt: Erstbeladung der Datenbank

### Beladen mit Oracle SQL Loader

Schreiben Sie ein Controlfile für die Dateien
*wienerlinien-ogd-linien.csv*, *wienerlinien-ogd-haltestellen.csv* und
*wienerlinien-ogd-steige.csv*, die diesen Datenbestand in die Tabellen
Linie, Haltestelle und Steig lädt. Achten Sie auf die Reihenfolge beim
Ausführen der Befehle, durch die Fremdschlüsselbeziehungen muss zuerst
Linie und Haltestelle, danach erst die Tabelle Steig beladen werden.

## 3. Schritt: Aktualisieren der Daten bei einem neuen Beladen

Nun kann es vorkommen, dass die Linienführung geändert wird (siehe 14A,
der verlegt wurde). Da Sie immer die Originaldateien aus dem Netz
laden, müssen Sie bei einem vorhandenen Datenbestand folgendes beachten:

1. Linien, die neu hinzugekommen sind, werden eingefügt.
2. Haltestellen, die neu hinzugekommen sind, werden eingefügt.
3. Die Tabelle Steig wird geleert und neu beladen. Das ist möglich, da
   es keine Fremdschlüsselbeziehung auf die Tabelle Steig gibt.
4. Linien und Haltestellen, die nicht mehr vorkommen, müssen gelöscht
   werden.

Überlegen Sie sich, ob diese Reihenfolge verpflichtend ist oder ob auch
eine andere Reihenfolge möglich ist.

### Neubeladen mit Oracle SQL Loader

Da Sie nicht direkt die Haupttabellen beladen können (referentielle
Integrität), erstellen Sie 3 Tabellen: Tabelle *LinieStage*,
*HaltestelleStage* und *SteigStage* in der Oracle Datenbank. Diese
Tabellen werden immer von den entsprechenden Textdateien mit *REPLACE*
beladen.

Schreiben Sie nun ein Shellskript, welches die oberen Punkte durch die
entsprechenden SQL Loader Befehle abbildet. Sie erhalten am Ende die
beladenen Tabellen LinieStage, HaltstelleStage und SteigStage.

Damit die Daten übertragen werden, müssen Sie die Daten von den Stage
Tabellen in die Originaltabellen einfügen. Schreiben Sie dafür eine
PL/SQL Prozedur, welche die oben genannten Schritte mit entsprechenden
INSERT und DELETE Befehlen abbildet. Dadurch übertragen Sie die Daten
aus den Stage Tabellen und verletzen zu keinem Zeitpunkt die
referentielle Integrität.

## Testen, Testen, Testen

Führen Sie mehrere Tests durch. Da das Ändern der Textdateien mühsam
ist, können Sie in Ihrer Datenbank

- Eine Linie samt ihren Steigen löschen und danach das Importscript
  starten.
- Alle Steige löschen und danach das Importscript starten.
- Eine neue Linie einfügen und dieser z. B. alle Steige der Linie 14A
  geben (INSERT mit SELECT) und danach Importscript starten.
