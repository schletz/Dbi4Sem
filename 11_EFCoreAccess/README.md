# Zugriff auf Views und Stored Procedures mit einem OR Mapper

> **Hinweis:** Sie benötigen eine virtuelle Maschine mit Oracle 12 oder einen Docker Container
> mit Oracle 19 bzw. 21.  Anleitungen befinden sich im Kurs *Dbi2Sem* auf
> https://github.com/schletz/Dbi2Sem/tree/master/01_OracleVM (VM mit Oracle 12) und auf
> https://github.com/schletz/Dbi2Sem/tree/master/01_OracleVM/03_Docker (Oracle 21 als Docker image)

## Erstellen eines Users und Befüllen der Datenbank

Verbinden Sie sich mit dem User System SQL Developer oder DBeaver
(Anleitung im Kurs *Dbi2Sem* auf https://github.com/schletz/Dbi2Sem/tree/master/01_OracleVM/01_Dbeaver).

Als Ausgangsbasis verwenden wir die bei den analytischen Funktionen verwendete Sportfestdatenbank.
Erstellen Sie zuerst einen neuen User *Sportfest* mit folgenden Berechtigungen:

```sql
DROP USER Sportfest CASCADE;
CREATE USER Sportfest IDENTIFIED BY oracle;
GRANT CONNECT, RESOURCE, CREATE VIEW TO Sportfest;
GRANT UNLIMITED TABLESPACE TO Sportfest;
```

Die erste Zeile (*DROP USER*) wird beim ersten Ausführen fehlschlagen, da der User noch nicht
existiert. Überspringen Sie mit *Skip* diesen Befehl.

Verbinden Sie sich nun mit dem User Sportfest und kopieren das
[SQL Skript aus dem Kapitel Analytische Funktionen](sportfest.sql)
in das Abfragefenster.

## Erstellen einer Konsolenapplikation mit EF Core

Geben Sie nun in der Konsole in ein Verzeichnis Ihrer Wahl. Führen Sie danach die folgenden
Befehle aus. Unter Linux oder macOS müssen die md und rd Befehle angepasst werden.

```text
rd /S /Q SportfestApp
md SportfestApp
cd SportfestApp
md SportfestApp.Application
cd SportfestApp.Application
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 6.*
dotnet add package Oracle.EntityFrameworkCore --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 6.*
cd ..
md SportfestApp.Test
cd SportfestApp.Test
dotnet new xunit
dotnet add reference ..\SportfestApp.Application
cd ..
md SportfestApp.ConsoleApp
cd SportfestApp.ConsoleApp
dotnet new console
dotnet add reference ..\SportfestApp.Application
cd ..
dotnet new sln
dotnet sln add SportfestApp.ConsoleApp
dotnet sln add SportfestApp.Application
dotnet sln add SportfestApp.Test
start SportfestApp.sln
```

Öffnen Sie nun die Datei SportfestApp.sln in Visual Studio. In der Konsole können Sie dies mit
*start SportfestApp.sln* am Schnellsten erledigen.

## Erstellen und Nutzen einer View

Erstellen Sie in der Datenbank unter dem User *Sportfest* folgende View und prüfen Sie das Ergebnis:

```sql
CREATE OR REPLACE VIEW vBewerbe AS
SELECT E_Bewerb AS B_Name, COUNT(*) AS B_Count
FROM Ergebnisse
GROUP BY E_Bewerb;
SELECT * FROM vBewerbe;
```

Sie sehen nun folgende Ausgabe:

| B_NAME    | B_COUNT |
| --------- | ------- |
| 100m Lauf | 217     |
| 5km Lauf  | 197     |
| 400m Lauf | 199     |


Nun wollen wir auf diese View mittels EF Core zugreifen.


### Erstellen der Modelklasse und des DbContextes

Unsere View liefert wie eine Tabelle ein Ergebnis (ein Resultset) an das Programm. Damit daraus
eine zugreifbare Klasse wird, legen wir eine entsprechende Modelklasse und eine
Contextklasse im Application Projekt an:

**Bewerb.cs**
```c#
public record Bewerb(
    [property: Column("B_NAME")] string Name,   // Oracle: uppercase
    [property: Column("B_COUNT")] int Count);

```

**SportfestContext.cs**
```c#
public class SportfestContext : DbContext
{
    public SportfestContext(DbContextOptions opt) : base(opt) { }
    public IQueryable<Bewerb> Bewerbe => Set<Bewerb>();  // read-only access. No .Add() or .Remove() method.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bewerb>()
            .HasNoKey()
            .ToView("VBEWERBE");   // Oracle: uppercase
    }
}
```

Es fallen folgende Dinge auf:

- **Positional record:** Ein positional record in C# 9 erstellt eine Klasse mit read-only
  Properties, die den Argumenten im Konstruktor entsprechen. Da wir in einer View die Daten
  nicht ändern können, eignet sich dieses Feature sehr gut für die Definition unserer Modelklasse.
- **Column annotations:** Die generierten Properties können mit Annotations versehen werden.
  Wir verwenden die Column Annotation, um die Argumente auf die Felder der View zu mappen.
  Oracle verlangt die Großschreibung, deswegen werden die Feldnamen hier großgeschrieben.
- **OnModelCreating:** Da unsere View keine Datenbanktabelle ist, besitzt sie auch keinen primary
  key. Mit *HasNoKey()* müssen wir dies definieren. Zudem mappen wir mit *ToView()* unsere
  Klasse zur View in der Datenbank.
- **IQueryable:** Ein DbSet unterstützt auch Einfüge- und Löschoperationen. Da wir dies bei einer
  View nicht brauchen, geben wir lediglich das Abfrageinterface *IQueryable* zurück.

> Hinweis: Views können durch INSTEAD OF Trigger auch Änderungen sowie Einfüge- und Löschoperationen
> unterstützen. In diesem Fall kann auch eine normale Klasse mit public set Properties und ein DbSet
> verwendet werden.

Da im record keine komplexen Typen verwendet werden, kann EF Core auch ohne Default Konstruktor
mit dieser Klasse arbeiten.

Um den Zugriff zu testen, legen wir im Test Projekt eine Klasse *SportfestContextTests* an.

**SportfestContextTests.cs**
```c#
public class SportfestContextTests
{
    public static readonly DbContextOptions _options = new DbContextOptionsBuilder()
        .UseOracle($"User Id=Sportfest;Password=oracle;Data Source=localhost:1521/XEPDB1")
        .Options;

    [Fact]
    public void ReadViewSuccessTest()
    {
        using var db = new SportfestContext(_options);
        var bewerbe = db.Bewerbe.ToList();
        Assert.True(bewerbe.Count > 0);
    }
}
```

Hier sehen wir den *Verbindungsstring* zur Oracle Datenbank. Er hat einen ähnlichen Aufbau
wie die Strings für SQL Server.

> **Hinweis:** Bei Oracle 12 ist der Service Name *XEPDB1* durch *ORCL* zu ersetzen.

## Zugreifen auf Stored Procedures

Legen Sie in der Sportfestdatenbank eine Prozedur mit dem Namen *get_results* an. Diese
Prozedur liest Werte aus der Datenbank in einen Cursor. Dieser Cursor wird als*OUT* Parameter
an unsere Applikation geliefert.

```sql
CREATE OR REPLACE
PROCEDURE get_results (bewerb     IN  Ergebnisse.E_Bewerb%TYPE,
                      p_recordset OUT SYS_REFCURSOR) AS
BEGIN
  OPEN p_recordset FOR
    SELECT e.E_ID, s.S_ID, s.S_ZUNAME, s.S_ABTEILUNG, s.S_KLASSE, e.E_BEWERB, e.E_ZEIT
    FROM   Ergebnisse e INNER JOIN Schueler s ON (e.E_SCHUELER = s.S_ID)
    WHERE  E_Bewerb = bewerb
    ORDER BY E_Schueler;
END;
```

Beim Aufruf der Prozedur im SQL Editor sehen wir einen seltsamen Rückgabewert:

```sql
CALL get_results('100m Lauf',?);
```

| P_RECORDSET |
| ----------- |
| REFCURSOR   |

Die Prozedur öffnet nämlich einen Cursor und befüllt ihn mit dem Ergebnis des Statements
`SELECT ... FROM Ergebnisse WHERE E_Bewerb=bewerb ORDER BY E_Schueler;` 

### Erstellen der Modelklasse und einer Methode im DbContext

Würden wir das SQL Statement, die die Prozedur als Cursor liefert, ausführen, wäre das
Ergebnis wie folgt:

| E_ID | S_ID | S_ZUNAME   | S_ABTEILUNG | S_KLASSE | E_BEWERB  | E_ZEIT    |
| ---- | ---- | ---------- | ----------- | -------- | --------- | --------- |
| 1011 | 1001 | Zuname1001 | HIF         | 1AHIF    | 400m Lauf | 78.4594   |
| 1048 | 1001 | Zuname1001 | HIF         | 1AHIF    | 400m Lauf | 69.4872   |
| 1056 | 1001 | Zuname1001 | HIF         | 1AHIF    | 5km Lauf  | 1589.6944 |
| 1084 | 1001 | Zuname1001 | HIF         | 1AHIF    | 5km Lauf  | 1555.0421 |

Wir müssen daher wieder eine Modelklasse erstellen, die das Ergebnis dieser Prozedur
abbildet.

**Ergebnis.cs**
```c#
public record Ergebnis(
    [property: Column("E_ID")] int ErgebnisNr,
    [property: Column("S_ID")] int SchuelerNr,
    [property: Column("S_ZUNAME")] string Zuname,
    [property: Column("S_ABTEILUNG")] string Abteilung,
    [property: Column("S_KLASSE")] string Klasse,
    [property: Column("E_BEWERB")] string Bewerb,
    [property: Column("E_ZEIT")] decimal Zeit
);
```

Unseren Datenbankcontext müssen wir jetzt mit 2 Dingen erweitern: Einerseits muss die Modelklasse
In *OnModelCreating()* wieder als keyless Entity konfiguriert werden. Andererseits ist eine Prozedur
ja eine Methode, die Parameter verarbeitet.
Diese müssen wir aus unserem C# Programm mitgeben. Daher legen wir auch eine Methode mit dem
Namen *GetResults()* an. Sie bekommt die Argumente für die Prozedur und reicht diese weiter.

```c#
public class SportfestContext : DbContext
{
    public SportfestContext(DbContextOptions opt) : base(opt) { }
    public IQueryable<Bewerb> Bewerbe => Set<Bewerb>();  // read-only access. No .Add() or .Remove() method.
    public IQueryable<Ergebnis> GetResults(string bewerb) =>
        Set<Ergebnis>().FromSqlRaw("BEGIN get_results(:bewerb, :result); END;",
                            new OracleParameter("bewerb", bewerb),
                            new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bewerb>()
            .HasNoKey()
            .ToView("VBEWERBE");   // Oracle: uppercase

        modelBuilder.Entity<Ergebnis>()
            .HasNoKey();
    }
}
```

Zum Schluss erstellen wir noch in der Klasse *SportfestContextTests* im Testprojekt eine
Testmethode *ReadProcedureSuccessTest()*.

```c#
public class SportfestContextTests
{
    public static readonly DbContextOptions _options = new DbContextOptionsBuilder()
        .UseOracle($"User Id=Sportfest;Password=oracle;Data Source=localhost:1521/XEPDB1")
        .Options;

    [Fact]
    public void ReadViewSuccessTest()
    {
        using var db = new SportfestContext(_options);
        var bewerbe = db.Bewerbe.ToList();
        Assert.True(bewerbe.Count > 0);
    }
    [Fact]
    public void ReadProcedureSuccessTest()
    {
        using var db = new SportfestContext(_options);
        var ergebnisse = db.GetResults("100m Lauf").ToList();
        Assert.True(ergebnisse.Count > 0);
    }
}
```

## Übung

In der Solution gibt es noch ein Projekt, welches wir nicht bearbeitet haben: Das *ConsoleApp*
Projekt. Sie soll folgendes umsetzen:

- Beim Starten des Programmes sollen aus der angelegten View *vBewerbe* die Bewerbe und die
  Anzahl der Ergebnisse gelesen werden. Listen Sie diese Bewerbe nach der Anzahl der Ergebnisse
  sortiert in der Konsole auf.
- Geben Sie darunter eine Zeile mit dem Text *Geben Sie den Bewerb ein: * aus.
- Der User kann dann den Namen eines Bewerbes eingeben.
- Diese Eingabe verwenden Sie, um über die angelegte Prozedur *get_results* die Liste der
  Ergebnisse nach Klasse und Schülernamen sortiert auszugeben.

Eine mögliche Darstellung ist die Folgende (100m Lauf ist eine Eingabe):

```text
BEWERBE IN DER DATENBANK
   100m Lauf (217 Ergebnisse)
   400m Lauf (199 Ergebnisse)
   5km Lauf (197 Ergebnisse)

Geben Sie einen Bewerb ein: 100m Lauf

ERGEBNISSE DES BEWERBES 100m Lauf
   1368	Zuname1011, Klasse 1AFIT: 18.5175 Sekunden
   1367	Zuname1011, Klasse 1AFIT: 16.7377 Sekunden
```

Du kannst natürlich auch eine ASP.NET Core Webapplikation erstellen, die diese Daten als
HTML View ausgibt. Dies ist der Vorteil des OR Mappers und eines Application Projektes: Der
Zugriff erfolgt vollkommen unabhängig von der verwendeten Datenbank.
