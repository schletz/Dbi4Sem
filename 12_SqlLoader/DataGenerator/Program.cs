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
        static readonly string sysUser = "System";
        static readonly string sysPassword = "oracle";
        static int verkaufCount = 10000;

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;

            args = args.Concat(new string[] { verkaufCount.ToString() }).ToArray();
            int.TryParse(args[0], out verkaufCount);

            Console.WriteLine("Erstelle den User Fahrkarten...");
            using (FahrkartenContext db = new FahrkartenContext($"User Id={sysUser};Password={sysPassword};Data Source=localhost:1521/orcl"))
            {
                try { db.Database.ExecuteSqlCommand("DROP USER Fahrkarten CASCADE"); }
                catch { }
                db.Database.ExecuteSqlCommand("CREATE USER Fahrkarten IDENTIFIED BY oracle");
                db.Database.ExecuteSqlCommand("GRANT CONNECT, RESOURCE, CREATE VIEW TO Fahrkarten");
                db.Database.ExecuteSqlCommand("GRANT UNLIMITED TABLESPACE TO Fahrkarten");
            }


            Station[] stations = new Station[]
            {
                new Station { StationId = 1000, Name = "Meidling" },
                new Station { StationId = 1001, Name = "Westbahnhof" },
                new Station { StationId = 1002, Name = "Längenfeldgasse" },
                new Station { StationId = 1003, Name = "Praterstern" }
            };

            Kartenart[] kartenarts = new Kartenart[]
            {
                new Kartenart{KartenartId = 1000, Name = "1 Fahrt WIEN", TageGueltig = null, Preis = 2.4M},
                new Kartenart{KartenartId = 1001, Name = "1 Tag WIEN", TageGueltig = 1, Preis = 8M},
                new Kartenart{KartenartId = 1002, Name = "Wochenkarte", TageGueltig = 7, Preis = 17.10M},
                new Kartenart{KartenartId = 1003, Name = "Monatskarte", TageGueltig = 30, Preis = null}
            };

            Randomizer.Seed = new Random(2150805);

            int[] hours = Enumerable.Range(0, 24).ToArray();
            float[] hoursWeights = new float[] { 1, 1, 1, 1, 2, 3, 5, 7, 8, 6, 4, 2, 2, 2, 2, 2, 2, 4, 5, 4, 2, 1, 1, 1 }
                .Select(i => i / 69f).ToArray();

            int[] weekdays = Enumerable.Range(0, 7).ToArray();
            float[] daysWeight = new float[] { 8, 6, 4, 4, 6, 2, 1 }
                .Select(i => i / 31f).ToArray();

            float[] stationsWeight = new float[] { 5, 4, 1, 2 }.Select(w => w / 12f).ToArray();
            float[] kartenartWeight = new float[] { 32, 16, 8, 4 }.Select(w => w / 60f).ToArray();
            int verkaufId = 1000;
            var verkaufFaker = new Faker<Verkauf>()
                .Rules((f, v) =>
                {
                    int hour = f.Random.WeightedRandom(hours, hoursWeights);
                    int weekday = f.Random.WeightedRandom(weekdays, daysWeight);
                    v.VerkaufId = verkaufId++;
                    v.Datum = new DateTime(2018, 12, 31)
                        .AddDays(7 * f.Random.Int(0, 52) + f.Random.WeightedRandom(weekdays, daysWeight))
                        .AddHours(f.Random.WeightedRandom(hours, hoursWeights) + f.Random.Int(0,3600)/3600.0);
                    v.Wochentag = ((int)v.Datum.DayOfWeek == 0) ? 7 : (int)v.Datum.DayOfWeek;
                    v.Stunde = v.Datum.Hour;
                    v.KartenartNavigation = f.Random.WeightedRandom(kartenarts, kartenartWeight);
                    v.StationNavigation = f.Random.WeightedRandom(stations, stationsWeight);
                });
            List<Verkauf> verkaufs = verkaufFaker.Generate(verkaufCount);

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Erstelle die Datenbank...");
            using (FahrkartenContext db = new FahrkartenContext($"User Id=Fahrkarten;Password=oracle;Data Source=localhost:1521/orcl"))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                sw.Start();
                db.Stations.AddRange(stations);
                db.Kartenarts.AddRange(kartenarts);
                db.Verkaufs.AddRange(verkaufs);
                db.SaveChanges();
                sw.Stop();
            }
            Console.WriteLine($"{verkaufCount} Verkäufe in {sw.ElapsedMilliseconds / 1000:0.0} s eingefügt.");

            Console.WriteLine("Schreibe die TXT Dateien...");
            using (var fileStream = new StreamWriter("stations.txt", false, Encoding.UTF8))
            {
                fileStream.WriteLine(string.Join("\t", "ID", "NAME"));
                foreach (var s in stations)
                    fileStream.WriteLine(string.Join("\t", s.StationId, s.Name));
            }
            using (var fileStream = new StreamWriter("kartenarten.txt", false, Encoding.UTF8))
            {
                fileStream.WriteLine(string.Join("\t", "ID", "NAME", "TAGEGUELTIG", "PREIS"));
                foreach (var k in kartenarts)
                    fileStream.WriteLine(string.Join("\t", 
                        k.KartenartId,
                        k.Name,
                        k.TageGueltig?.ToString() ?? "N/A",
                        k.Preis));
            }

            var fileStreams = new StreamWriter[3];
            using (fileStreams[0] = new StreamWriter("verkaeufe1.txt", false, Encoding.UTF8))
            using (fileStreams[1] = new StreamWriter("verkaeufe2.txt", false, Encoding.UTF8))
            using (fileStreams[2] = new StreamWriter("verkaeufe3.txt", false, Encoding.UTF8))
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
