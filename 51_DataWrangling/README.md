# Data Wrangling mit dem Dataframe

**[Zum Cheat Sheet](https://pandas.pydata.org/Pandas_Cheat_Sheet.pdf)**

In der Übung *IncrementalLoad* mit SQL*Loader haben wir 3 Textdateien der Wiener Linien gelesen:

- *csv-linien*: https://data.wien.gv.at/csv/wienerlinien-ogd-linien.csv
- *csv-haltestellen*: https://data.wien.gv.at/csv/wienerlinien-ogd-haltestellen.csv
- *csv-steige*: https://data.wien.gv.at/csv/wienerlinien-ogd-steige.csv

Nun wollen wir diese Dateien direkt über HTTP anfordern, Auswertungen anstellen und das Ergebnis
präsentieren.

### Gruppierung ([wienerlinien1.ipynb](wienerlinien1.ipynb))

Im Notebook [wienerlinien1.ipynb](wienerlinien1.ipynb) wird das Thema Gruppierung von Daten behandelt.

### Joins und Index ([wienerlinien2.ipynb](wienerlinien2.ipynb))

Im Notebook [wienerlinien2.ipynb](wienerlinien2.ipynb) wird das Thema Joins und Index behandelt.

## Übung ([erreichbarkeit.ipynb](erreichbarkeit.ipynb))

Im Notebook [erreichbarkeit.ipynb](erreichbarkeit.ipynb) werden diese Daten samt dem Adressregister
geladen. Führen Sie eine Auswertung durch, wie viel % der Adressen (und somit näherungsweise der
Prozentanteil der Bevölkerung) in Wien eine U Bahn Station in der Nähe hat. Das Ergebnis ist je nach Bezirk
sehr unterschiedlich. Durch die Verwendung von Samples kann Ihr Ergebnis geringfügig abweichen.

|    | PLZ  | IS_REACHABLE |
|----|------|--------------|
| 0  | 1010 | 100.00       |
| 1  | 1020 | 95.92        |
| 2  | 1030 | 91.34        |
| 3  | 1040 | 100.00       |
| 4  | 1050 | 86.62        |
| 5  | 1060 | 100.00       |
| 6  | 1070 | 100.00       |
| 7  | 1080 | 100.00       |
| 8  | 1090 | 100.00       |
| 9  | 1100 | 67.37        |
| 10 | 1110 | 45.12        |
| 11 | 1120 | 70.72        |
| 12 | 1130 | 27.77        |
| 13 | 1140 | 33.12        |
| 14 | 1150 | 100.00       |
| 15 | 1160 | 69.52        |
| 16 | 1170 | 21.85        |
| 17 | 1180 | 39.60        |
| 18 | 1190 | 21.05        |
| 19 | 1200 | 98.77        |
| 20 | 1210 | 28.87        |
| 21 | 1220 | 46.78        |
| 22 | 1230 | 27.07        |