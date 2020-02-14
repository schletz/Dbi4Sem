# DBI im 4. Semester

## Lehrinhalte

- [Data Warehouse](01_DataWarehouse)
  1. [Zugriff auf die Oracle Datenbank mit EF Core](01_DataWarehouse/01_EFCoreAccess.md)
  2. [Oracle SQL*Loader](01_DataWarehouse/02_SqlLoader.md)
  3. [Inkrementelles Laden](01_DataWarehouse/03_IncrementalLoad.md)
  4. [Lesen von JSON Daten](01_DataWarehouse/04_Json.md)
  5. [Lesen von Webquellen mit den Worker Services in .NET Core 3.0](01_DataWarehouse/05_ServiceWorker.md)
- Business Intelligence

## Synchronisieren des Repositories in einen Ordner

1. Laden Sie von https://git-scm.com/downloads die Git Tools (Button *Download 2.xx for Windows*)
    herunter. Es können alle Standardeinstellungen belassen werden, bei *Adjusting your PATH environment*
    muss aber der mittlere Punkt (*Git from the command line [...]*) ausgewählt sein.
2. Legen Sie einen Ordner auf der Festplatte an, wo die Daten gespeichert werden sollen
    (z. B. *C:\Schule\DBI\Repo*).
3. Initialisieren Sie den Ordner mit folgenden Befehlen, die in der Windows Konsole in diesem Verzeichnis
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
