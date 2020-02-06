# DBI im 4. Semester

## Lehrinhalte

- [Data Warehouse](tree/master/01_DataWarehouse)
- [Business Intelligence](tree/master/02_BusinessIntelligence)

## Synchronisieren des Repositories in einen Ordner

1. Laden Sie von https://git-scm.com/downloads die Git Tools (Button *Download 2.xx for Wiindows*)
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
