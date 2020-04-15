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
        private const string connection = @"Server=.\SQLSERVER2019;Database=Fahrkarten;Trusted_Connection=True;";
        static int verkaufCount = 10000;

        static void Main(string[] args)
        {
            Console.WriteLine("DATENGENERATOR FÜR VERKÄUFE");

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
                    v.VerkaufId = verkaufId++;
                    v.Menge = f.Random.Int(1, 5);
                    // Zeitwerte mit Wochen- und Tageszyklus generieren.
                    v.Datum = new DateTime(2018, 12, 31)
                        .AddDays(7 * f.Random.Int(0, 52) + f.Random.WeightedRandom(weekdays, daysWeight))
                        .AddHours(f.Random.WeightedRandom(hours, hoursWeights) + f.Random.Int(0, 3600) / 3600.0);
                    v.Kartenart = f.Random.WeightedRandom(kartenarts, kartenartWeight);
                    v.Station = f.Random.WeightedRandom(stations, stationsWeight);
                });
            List<Verkauf> verkaufs = verkaufFaker.Generate(verkaufCount);

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Erstelle die Datenbank...");

            using (FahrkartenContext db = new FahrkartenContext(connection))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Stationen.AddRange(stations);
                db.Kartenarten.AddRange(kartenarts);
                db.Verkaeufe.AddRange(verkaufs);
                sw.Start();
                db.SaveChanges();
                sw.Stop();
            }
            Console.WriteLine($"{verkaufCount} Verkäufe in {sw.ElapsedMilliseconds / 1000:0.0} s eingefügt.");
        }
    }
}
