# DBI im 6. Semester bzw. V. Jahrgang

## Lehrinhalte

<table>
    <tr>
        <th>1</th>
        <th>2</th>
        <th>3</th>
        <th>4</th>
        <th>5</th>
    </tr>
    <tr>
        <td colspan="2" valign="top">
            <strong>1 Oracle Datenbankadmin</strong>
            <ul>
                <li><a href="01_Datenbankadmin/01_Transactions.md">Transactions</a></li>
                <li><a href="01_Datenbankadmin/02_Instance.md">Instance</a></li>
                <li><a href="01_Datenbankadmin/03_Memory.md">Memory</a></li>
                <li><a href="01_Datenbankadmin/04_Locking.md">Locking</a></li>
                <li><a href="01_Datenbankadmin/05_Null.md">Null</a></li>
                <li><a href="01_Datenbankadmin/06_RegExp.md">Regular Expressions</a></li>
                <li><a href="01_Datenbankadmin/07_Rights.md">Rights</a></li>
                <li><a href="01_Datenbankadmin/08_Isolation.md">Isolation</a></li>
                <li><a href="01_Datenbankadmin/09_Process.md">Process</a></li>
            </ul>
        </td>
        <td colspan="3" valign="top">
            <strong>2 Data Warehouse</strong>
            <ul>
                <li><a href="11_EFCoreAccess/README.md">Zugriff auf Views und Stored Procedures mit einem OR Mapper</a>
                </li>
                <li><a href="12_SqlLoader/README.md">Oracle SQL*Loader</a></li>
                <li><a href="13_IncrementalLoad/README.md">Inkrementelles Laden</a></li>
            </ul>
            <strong>3 Business Intelligence</strong>
            <ul>
                <li><a href="41_SqlServerInstall/README.md">Installation der SQL Server 2019 Analysis Services</a></li>
                <li><a href="42_DatabaseGenerator/README.md">Generierung der Musterdatenbank, Zeitreihenanalyse</a></li>
                <li><a href="https://web.microsoftstream.com/video/ac28cc0a-25b2-4c2e-b2f3-4aa19d7ab054">Analysis
                        Services Intro (Video)</li>
                <li><a href="https://web.microsoftstream.com/video/c70ac6b2-8e29-42df-8311-745756c8d8d5">Time Dimensions
                        (Video)</a></li>
            </ul>
            <strong>4 Data Science mit Python und Pandas</strong>
            <ul>
                <li><a href="50_JupyterNotebooks/README.md">Intro zu Jupyter Notebooks</a></li>
                <li><a href="51_DataWrangling/README.md">Data Wrangling</a></li>
                <li><a href="52_DescriptiveStatistics/README.md">Deskriptive Statistik</a></li>
                <li><a href="56_ETL/README.md">ETL (Extract, Transform, Load)</a></li>
                <li><a href="53_Regression/README.md">Erste Modelle: Normalverteilung, Lognormalverteilung und Regressionsmodell</a></li>
                <li>Time Series</li>
                <li>Plots</li>
                <li><a href="57_Cubes/README.md">Cubes</a></li>
            </ul>
            <strong><a href="https://scikit-learn.org/stable/index.html" target="_blank">5 Machine Learning mit scikit learn</a></strong>
        </td>
    </tr>
</table>

## Voraussetzungen

- [Oracle Docker Image](https://github.com/schletz/Dbi2Sem/tree/master/01_OracleVM/03_Docker)


## Synchronisieren des Repositories in einen Ordner

1. Laden Sie von https://git-scm.com/downloads die Git Tools (Button *Download 2.23.0 for Windows*)
herunter. Es können alle Standardeinstellungen belassen werden, bei *Adjusting your PATH environment*
muss aber der mittlere Punkt (*Git from the command line [...]*) ausgewählt sein.
2. Lege einen Ordner auf der Festplatte an, wo Sie die Daten speichern möchten
(z. B. *C:\Schule\DBI*). Das
Repository ist nur die lokale Version des Repositories auf https://github.com/schletz/Dbi4Sem.git.
Hier werden keine Commits gemacht und alle lokalen Änderungen dort werden bei der
nächsten Synchronisation überschrieben.
3. Führen Sie in diesem Ordner den folgenden Befehl aus:

```text
git clone https://github.com/schletz/Dbi4Sem.git
```

Um Änderungen auf den Rechner zu Übertragen, können Sie die Datei *resetGit.cmd* ausführen.
Alle lokalen Änderungen gehen dabei verloren!