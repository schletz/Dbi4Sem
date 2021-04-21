# DBI im 4. Semester

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
        <td colspan="2" valign="top" rowspan="2">
            <b>1</b> XML und CMS Systeme
            <ol>
                <li>Aufbau von XML, XPath, XQuery, Schemata</li>
                <li>Joomla CMS System</li>
            </ol>
        </td>
        <td valign="top" rowspan="2">
            <b>2</b> NoSQL
        <td colspan="2" valign="top">
            <b>3</b> Data Warehouse
            <ol>
                <li><a href="11_EFCoreAccess/README.md">Zugriff auf die Oracle Datenbank mit EF Core</a></li>
                <li><a href="12_SqlLoader/README.md">Oracle SQL*Loader</a></li>
                <li><a href="13_IncrementalLoad/README.md">Inkrementelles Laden</a></li>
                <li><a href="14_Json/README.md">Lesen von JSON Daten</a></li>
            </ol>
        </td>
    </tr>
    <tr>
        <td>
            <b>4</b> Business Intelligence
            <ol>
                <li><a href="41_SqlServerInstall/README.md">Installation der SQL Server 2019 Analysis Services</a></li>
                <li><a href="42_DatabaseGenerator/README.md">Generierung der Musterdatenbank</a></li>
                <li><a href="https://web.microsoftstream.com/video/ac28cc0a-25b2-4c2e-b2f3-4aa19d7ab054">Analysis Services Intro (Video)</li>
                <li><a href="https://web.microsoftstream.com/video/c70ac6b2-8e29-42df-8311-745756c8d8d5">Time Dimensions (Video)</li>
            </ol>
        </td>
    </tr>
</table>

## Voraussetzungen

- [Installation der Oracle 12 VM](02_OracleVM/README.md)
- [Verwendung von Dbeaver mit Oracle](02_OracleVM/01_Dbeaver/README.md)

## Synchronisieren des Repositories in einen Ordner

1. Laden Sie von https://git-scm.com/downloads die Git Tools (Button *Download 2.xx for Windows*)
    herunter. Es können alle Standardeinstellungen belassen werden, bei *Adjusting your PATH environment*
    muss aber der mittlere Punkt (*Git from the command line [...]*) ausgewählt sein.
2. Schließen Sie die Konsole und starten Sie sie erneut, damit die Änderungen in PATH neu gelesen
    werden.
3. Legen Sie einen Ordner auf der Festplatte an, wo die Daten gespeichert werden sollen
    (z. B. *C:\Schule\DBI\Repo*).
4. Initialisieren Sie den Ordner mit folgenden Befehlen, die in der Windows Konsole in diesem Verzeichnis
    ausgeführt werden:

```text
git init
git remote add origin https://github.com/schletz/Dbi4Sem.git
git fetch --all
git reset --hard origin/master
```

### Nachträgliches Synchronisieren

Führen Sie die Datei *resetGit.cmd* aus. Dadurch werden die lokalen Änderungen zurückgesetzt und der
neue Stand wird vom Server übertragen.
