# Installation der SQL Server 2019 Analysis Services

> **Wichtig:** Starten Sie vor der Installation Windows neu. Ausstehende Updates, etc. verhindern
> ein erfolgreiches Setup!

Für den Zugriff auf die Analysis Services von SQL Server benötigen wir eine lokale Installation
von SQL Server 2019. Sie können die Installationsdatei der Developer Edition von der
[SQL Server Downloadseite](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
beziehen.

![](download.png)

## Installationsdialoge

Nach dem Starten des Installers wählen Sie *Custom*. Danach werden Sie nach dem Ort gefragt, wo die
Installationsdateien kopiert werden sollen. Dieser kann beliebig sein.

![](setup01.png)

Nach dem Download startet das Setup und Sie können eine neue SQL Server Instanz installieren:

![](setup02.png)

Die weiteren Dialoge können Sie einfach bestätigen, die Dialoge wo Einstellungen gemacht werden
müssen sind nachfolgend erklärt.

Bei den Features wählen Sie die Database Engine und die Analysis Services.

![](setup03.png)

Als Instanzname wählen Sie *SQLSERVER2019*, damit jeder den gleichen Namen hat.

![](setup04.png)

Für die Authentifizierung wählen Sie *Mixed Mode*. Dadurch können Sie mit Ihrem aktuellen Windows
Benutzer und mit dem User *sa* einsteigen. Da wir den SQL Server nicht nach außen freigeben, genügt
ein schwaches Kennwort (hier *1234*)

![](setup05.png)

Für die Analysis Services aktivieren wir den *Multidimensional and Data Mining Mode*. Er erlaubt
das Erstellen von CUBEs.

![](setup06.png)

## Installation der Visual Studio Extension

Um ein Analysis Service Projekt zu erstellen, starten Sie Visual Studio 2019 ohne ein neues Projekt
anzulegen:

![](visualstudio1.png)


Danach wählen Sie im Menü *Extensions* den Punkt *Manage Extensions*. Wenn Sie nach *Analysis* suchen,
können SIe die *Microsoft Analysis Services Projects* auswählen und herunterladen:

![](visualstudio2.png)

Erst nach dem Schließen von Visual Studio beginnt die Installation.

## Installation von SQL Server Management Studio

Um Ihre SQL Server Installation zu testen, laden Sie sich das SQL Server Management Studio von
der [Microsoft Downloadseite](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15).

Starten Sie nun das Management Studio, indem Sie *SSMS* im Startmenü eingeben. Dnach können Sie sich
mit dem Server *.\SQLSERVER2019* (oder dem gewählten Instanznamen) verbinden. Danach wählen Sie
*Connect - Analysis Servives* und verbinden sich mit Ihrer SQL Server Analysis Services Instanz,
die ebenfalls unter *.\SQLSERVER2019* erreichbar ist.

![](ssms01.png)
