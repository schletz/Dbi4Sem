using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Bogus;
using Bogus.Extensions;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("DATENGENERATOR FÜR VERKÄUFE");
            Console.WriteLine();
            var server = Input("Servername oder IP des SQL Servers", @"127.0.0.1,1433", checkExp: @"^[0-9\.\,]");
            var database = Input("Datenbank", @"Fahrkarten");
            var username = Input("Username", @"sa");
            var password = Input("Passwort", "SqlServer2019", checkExp: ".+");
            var verkaufCount = int.Parse(Input("Anzahl der Verkäufe", "1000000", checkExp: @"\d{1,9}"));

            var connection = $@"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate=true";
            // Setzt die CultureInfo, damit . als Dezimaltrennzeichen eingestellt wird.
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;

            // 4 Stationen generieren.
            Station[] stations = new Station[]
            {
                new Station { StationId = 1000, Name = "Meidling", Latitude = 48 + 10.5M/60, Longitude = 16 + 20.1M/60 },
                new Station { StationId = 1001, Name = "Westbahnhof", Latitude = 48 + 11.8M/60, Longitude = 16 + 20.3M/60 },
                new Station { StationId = 1002, Name = "Längenfeldgasse", Latitude = 48 + 11.1M/60, Longitude = 16 + 20.1M/60 },
                new Station { StationId = 1003, Name = "Praterstern", Latitude = 48 + 13.1M/60, Longitude = 16 + 23.5M/60 }
            };

            // 4 Kartenarten generieren.
            Kartenart[] kartenarts = new Kartenart[]
            {
                new Kartenart{KartenartId = 1000, Name = "1 Fahrt WIEN", TageGueltig = null, Preis = 2.4M},
                new Kartenart{KartenartId = 1001, Name = "1 Tag WIEN", TageGueltig = 1, Preis = 8M},
                new Kartenart{KartenartId = 1002, Name = "Wochenkarte", TageGueltig = 7, Preis = 17.10M},
                new Kartenart{KartenartId = 1003, Name = "Monatskarte", TageGueltig = 30, Preis = 41.20M}
            };

            // Damit immer die gleichen Daten pro Programmlauf generiert werden, initialisieren
            // wir mit einem fixen Wert. Sonst wird die Zeit genommen und die Daten sind
            // unterschiedlich und somit nicht vergleichbar.
            Randomizer.Seed = new Random(2150805);

            // Gewichtete Zufallsdaten für die Stunde. Am Meisten Verkäufe sind um 8h morgens.
            int[] hours = Enumerable.Range(0, 24).ToArray();
            float[] hoursWeights = new float[] { 1, 1, 1, 1, 2, 3, 5, 7, 8, 6, 4, 2, 2, 2, 2, 2, 2, 4, 5, 4, 2, 1, 1, 1 }
                .Select(i => i / 69f).ToArray();
            // Gewichtete Zufallsdaten für den Wochentag. Am Meisten Verkäufe sind MO und DI.
            int[] weekdays = Enumerable.Range(0, 7).ToArray();
            float[] daysWeight = new float[] { 8, 6, 4, 4, 6, 2, 1 }
                .Select(i => i / 31f).ToArray();
            // Gewichtete Zufallsdaten für die eingefügten 4 Stationen.
            float[] stationsWeight = new float[] { 5, 4, 1, 2 }.Select(w => w / 12f).ToArray();
            // Gewichtete Zufallsdaten für die eingefügten Kartenarten.        
            float[] kartenartWeight = new float[] { 32, 16, 8, 4 }.Select(w => w / 60f).ToArray();
            int verkaufId = 1000;

            var verkaufFaker = new Faker<Verkauf>()
                .Rules((f, v) =>
                {
                    v.VerkaufId = verkaufId++;
                    v.Menge = f.Random.Int(1, 5);
                    // Zeitwerte mit Wochen- und Tageszyklus generieren.
                    v.Datum = new DateTime(2018, 12, 31)
                        .AddDays(7 * f.Random.Int(0, 52) + f.Random.WeightedRandom(weekdays, daysWeight))
                        .AddHours(f.Random.WeightedRandom(hours, hoursWeights) + f.Random.Int(0, 3600) / 3600.0);
                    v.Kartenart = f.Random.WeightedRandom(kartenarts, kartenartWeight);
                    v.KartenartId = v.Kartenart.KartenartId;
                    v.Station = f.Random.WeightedRandom(stations, stationsWeight);
                    v.StationId = v.Station.StationId;
                });
            List<Verkauf> verkaufs = verkaufFaker.Generate(verkaufCount);

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Verbinde mit SQL Server. Connection String:");
            Console.WriteLine(connection);
            using (FahrkartenContext db = new FahrkartenContext(connection))
            {
                Console.WriteLine("Erstelle die Datenbank...");
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Console.WriteLine("Füge die Daten ein (Sample mit max. 10000 Verkäufe)...");
                db.Stationen.AddRange(stations);
                db.Kartenarten.AddRange(kartenarts);
                db.Verkaeufe.AddRange(verkaufs.Take(10000));
                sw.Start();
                db.SaveChanges();
                sw.Stop();
            }
            Console.WriteLine($"Sample von max. 10000 Verkäufen in {sw.ElapsedMilliseconds / 1000:0.0} s eingefügt.");

            // Unter Windows verwenden wir C:\Temp, da das Tempverzeichnis des Users in C:\Users
            // nicht für den SQL Server User zugreifbar ist.
            var tempPath = Environment.OSVersion.Platform == PlatformID.Win32NT ? @"C:\Temp" : "/tmp";
            Directory.CreateDirectory(tempPath);
            var filename = Path.Combine(tempPath, "verkauf_unicode.tsv");
            WriteVerkaufFile(verkaufs, filename);
            Console.WriteLine($"{verkaufCount} Verkäufe in die Datei {filename} geschrieben.");
            Console.WriteLine($"Um alle Verkäufe in die Tabelle Verkauf zu laden, kopiere sie in ein Verzeichnis auf das der SQL Server Container Zugriff hat.");
            Console.WriteLine($"Führe folgenden Befehl im SQL Editor aus. /host ist das Volume, wo die tsv Datei liegt:");
            Console.WriteLine($@"
USE Fahrkarten;            
TRUNCATE TABLE Verkauf;	
BULK INSERT Verkauf
FROM '/host/verkauf_unicode.tsv' WITH (
        CODEPAGE  = 'RAW',
        FIRSTROW = 1,
        FIELDTERMINATOR = '\t'
);
SELECT COUNT(*) FROM Verkauf v; 
");

        }

        private static void WriteVerkaufFile(List<Verkauf> verkaufs, string filename)
        {
            using var verkaufStream = new StreamWriter(filename, false, Encoding.Unicode);
            for (int i = 0; i < verkaufs.Count; i++)
            {
                Verkauf v = verkaufs[i];
                verkaufStream.WriteLine($"{v.VerkaufId}\t{v.Datum:yyyy-MM-ddTHH:mm:ss}\t{v.Menge}\t{v.KartenartId}\t{v.StationId}");
            }
        }

        private static string Input(string message, string defaultVal = "", string checkExp = ".*")
        {
            var checkRegex = new System.Text.RegularExpressions.Regex(checkExp, System.Text.RegularExpressions.RegexOptions.Compiled);
            while (true)
            {
                Console.Write($"{message} ");
                if (!string.IsNullOrEmpty(defaultVal)) { Console.Write($"[{defaultVal}] "); }
                var val = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(defaultVal)) { return defaultVal; }
                if (checkRegex.IsMatch(val)) { return val; }
                Console.WriteLine("Ungültige Eingabe!");
            }
        }
    }
}
