using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sportfest.Model;
using System;

namespace Sportfest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setzt die CultureInfo, damit . als Dezimaltrennzeichen eingestellt wird.
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;

            // *************************************************************************************
            // 1. User Sportfest in Oracle erzeugen
            // *************************************************************************************
            Console.WriteLine("Verbinde als User System (Passwort oracle) zur Oracle DB und lege den User Sportfest an.");
            var oracleAdmin = new DbContextOptionsBuilder<SportfestContext>()
                .UseOracle($"User Id=System;Password=oracle;Data Source=localhost:1521/orcl")
                .Options;
            using (SportfestContext db = new SportfestContext(oracleAdmin))
            {
                try { db.Database.ExecuteSqlCommand("DROP USER Sportfest CASCADE"); }
                catch { }
                db.Database.ExecuteSqlCommand("CREATE USER Sportfest IDENTIFIED BY oracle");
                db.Database.ExecuteSqlCommand("GRANT CONNECT, RESOURCE, CREATE VIEW TO Sportfest");
                db.Database.ExecuteSqlCommand("GRANT UNLIMITED TABLESPACE TO Sportfest");
            }

            // *************************************************************************************
            // 2. Die Tabellen in Oracle unter Sportfest anlegen und Daten einspielen
            // *************************************************************************************
            Console.WriteLine("Verbinde als User Sportfest (Passwort oracle) zur Oracle DB und lege die Tabellen an.");
            var oracleSportfest = new DbContextOptionsBuilder<SportfestContext>()
                .UseOracle($"User Id=Sportfest;Password=oracle;Data Source=localhost:1521/orcl")
                .Options;
            using (SportfestContext db = new SportfestContext(oracleSportfest))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Seed(writeFile: true);
            }

            // *************************************************************************************
            // 3. Die Tabellen in SQL Server (Datenbank Sportfest) anlegen und Daten einspielen
            // *************************************************************************************
            Console.WriteLine("Verbinde mit dem Windows User zu SQL Server und lege die DB Sportfest an.");
            var sqlserverConn = new DbContextOptionsBuilder<SportfestContext>()
                .UseSqlServer(@"Server=.\SQLSERVER2019;Database=Sportfest;Trusted_Connection=True;")
                .Options;
            using (SportfestContext db = new SportfestContext(sqlserverConn))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Seed();
            }

            Console.WriteLine("Gratulation, es hat alles funktioniert :)))");
        }
    }
}
