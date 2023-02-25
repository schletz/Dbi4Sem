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
        static string sysUser = "System";
        static string sysPassword = "oracle";
        static int verkaufCount = 10000;

        static void Main(string[] args)
        {
            Console.WriteLine("DATENGENERATOR FÜR VERKÄUFE");
            Console.WriteLine("DataGenerator [Anz Verkäufe] [Sys Username] [Sys Passwort]");

            // Setzt die CultureInfo, damit . als Dezimaltrennzeichen eingestellt wird.
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;

            // Standardwerte für die Parameter definieren, falls sie nicht angegeben wurden.
            string[] param = new string[] { verkaufCount.ToString(), sysUser, sysPassword };
            args.CopyTo(param, 0);
            (verkaufCount, sysUser, sysPassword) = (int.Parse(param[0]), param[1], param[2]);

            // Mit dem system User (oben definiert) verbinden und den User Fahrkarten (Passwort oracle)
            // anlegen.
            var serviceName = "xepdb1";
            Console.WriteLine("Erstelle den User Fahrkarten...");
            using (FahrkartenContext db = new FahrkartenContext($"User Id={sysUser};Password={sysPassword};Data Source=localhost:1521/{serviceName}"))
            {
                try { db.Database.ExecuteSqlRaw("DROP USER Fahrkarten CASCADE"); }
                catch { }
                db.Database.ExecuteSqlRaw("CREATE USER Fahrkarten IDENTIFIED BY oracle");
                db.Database.ExecuteSqlRaw("GRANT CONNECT, RESOURCE, CREATE VIEW TO Fahrkarten");
                db.Database.ExecuteSqlRaw("GRANT UNLIMITED TABLESPACE TO Fahrkarten");
            }
            Console.WriteLine("*************************************************************************");
            Console.WriteLine("Du kannst dich nun mit folgenden Daten zur erstellen Oracle DB verbinden:");
            Console.WriteLine($"   Username:     Fahrkarten");
            Console.WriteLine($"   Passwort:     oracle");
            Console.WriteLine($"   Service Name: {serviceName}");
            Console.WriteLine("*************************************************************************");

            Console.WriteLine("Generiere Daten und schreibe in die Datenbank...");
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
                new Kartenart{KartenartId = 1003, Name = "Monatskarte", TageGueltig = 30, Preis = null}
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
                    int hour = f.Random.WeightedRandom(hours, hoursWeights);
                    int weekday = f.Random.WeightedRandom(weekdays, daysWeight);
                    v.VerkaufId = verkaufId++;
                    // Zeitwerte mit Wochen- und Tageszyklus generieren.
                    v.Datum = new DateTime(2018, 12, 31)
                        .AddDays(7 * f.Random.Int(0, 52) + f.Random.WeightedRandom(weekdays, daysWeight))
                        .AddHours(f.Random.WeightedRandom(hours, hoursWeights) + f.Random.Int(0, 3600) / 3600.0);
                    v.Wochentag = ((int)v.Datum.DayOfWeek == 0) ? 7 : (int)v.Datum.DayOfWeek;
                    v.Stunde = v.Datum.Hour;
                    v.KartenartNavigation = f.Random.WeightedRandom(kartenarts, kartenartWeight);
                    v.StationNavigation = f.Random.WeightedRandom(stations, stationsWeight);
                });
            List<Verkauf> verkaufs = verkaufFaker.Generate(verkaufCount);

            Stopwatch sw = new Stopwatch();
            using (FahrkartenContext db = new FahrkartenContext($"User Id=Fahrkarten;Password=oracle;Data Source=localhost:1521/{serviceName}"))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Stations.AddRange(stations);
                db.Kartenarts.AddRange(kartenarts);
                db.Verkaufs.AddRange(verkaufs);
                sw.Start();
                db.SaveChanges();
                sw.Stop();
            }
            Console.WriteLine($"{verkaufCount} Verkäufe in {sw.ElapsedMilliseconds / 1000:0.0} s eingefügt.");

            // Die Daten UTF8 codiert schreiben. Der Zeilenumbruch wird auf CR+LF festgelegt.
            Console.WriteLine("Schreibe die TXT Dateien...");
            using (var fileStream = new StreamWriter("stations.txt", false, new UTF8Encoding(false)) { NewLine = "\r\n"})
            {
                fileStream.WriteLine(string.Join("\t", "ID", "NAME", "LAT", "LON"));
                foreach (var s in stations)
                    fileStream.WriteLine(string.Join("\t", s.StationId, s.Name, s.Latitude, s.Longitude));
            }
            using (var fileStream = new StreamWriter("kartenarten.txt", false, new UTF8Encoding(false)) { NewLine = "\r\n" })
            {
                fileStream.WriteLine(string.Join("\t", "ID", "NAME", "TAGEGUELTIG", "PREIS"));
                foreach (var k in kartenarts)
                    fileStream.WriteLine(string.Join("\t",
                        k.KartenartId,
                        k.Name,
                        // Speziellen NULL Wert zur Demonstration im control file generieren.
                        k.TageGueltig?.ToString() ?? "N/A",
                        k.Preis));
            }

            // Die Verkäufe werden in 3 Dateien aufgeteilt, um den Import im control file zeigen
            // zu können.
            var fileStreams = new StreamWriter[3];
            using (fileStreams[0] = new StreamWriter("verkaeufe1.txt", false, new UTF8Encoding(false)) { NewLine = "\r\n" })
            using (fileStreams[1] = new StreamWriter("verkaeufe2.txt", false, new UTF8Encoding(false)) { NewLine = "\r\n" })
            using (fileStreams[2] = new StreamWriter("verkaeufe3.txt", false, new UTF8Encoding(false)) { NewLine = "\r\n" })
            {
                int i = 0;
                foreach (var fileStream in fileStreams)
                    fileStream.WriteLine(string.Join("\t", "ID", "DATUM", "TAG", "STUNDE", "STATION", "KARTENART"));

                foreach (var v in verkaufs)
                {
                    var currentStream = fileStreams[i++ % 3];
                    currentStream.WriteLine(string.Join("\t",
                        v.VerkaufId, v.Datum.ToString("yyyy-MM-ddTHH:mm:ss"), v.Wochentag,
                        v.Stunde, v.Station, v.Kartenart));
                }
            }
        }
    }
}
