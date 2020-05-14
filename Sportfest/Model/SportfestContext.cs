using Bogus;
using Bogus.DataSets;
using Bogus.Distributions.Gaussian;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sportfest.Model
{
    public class SportfestContext : DbContext
    {
        public DbSet<Bewerb> Bewerbe { get; set; }
        public DbSet<Ergebnis> Ergebnisse { get; set; }
        public DbSet<Klasse> Klassen { get; set; }
        public DbSet<Schueler> Schueler { get; set; }

        public SportfestContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Zusammengesetzten Schlüssel erzeugen
            modelBuilder.Entity<Ergebnis>().HasKey("SchuelerId", "BewerbId", "Durchgang");
            // Indizes erzeugen
            modelBuilder.Entity<Bewerb>().HasIndex(b => b.Name).IsUnique();

            // Für Oracle alle Namen großschreiben, sonst sind sie Case Sensitive und brauchen
            // ein " bei den Abfragen.
            if (Database.IsOracle())
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entity.GetProperties())
                    {
                        property.Relational().ColumnName = property.Relational().ColumnName.ToUpper();
                    }
                    entity.Relational().TableName = entity.Relational().TableName.ToUpper();
                }
            }
        }

        public void Seed(bool writeFile = false)
        {
            Randomizer.Seed = new Random(152111);
            Faker f = new Faker();
            var klassen = new List<Klasse>
            {
                new Klasse{Name = "1AHIF", Abteilung = "HIF", Jahrgang = 1},
                new Klasse{Name = "1BHIF", Abteilung = "HIF", Jahrgang = 1},
                new Klasse{Name = "1CHIF", Abteilung = "HIF", Jahrgang = 1},
                new Klasse{Name = "2AHIF", Abteilung = "HIF", Jahrgang = 2},
                new Klasse{Name = "2BHIF", Abteilung = "HIF", Jahrgang = 2},
                new Klasse{Name = "2CHIF", Abteilung = "HIF", Jahrgang = 2},
                new Klasse{Name = "1AHBGM", Abteilung = "HBGM", Jahrgang = 1},
                new Klasse{Name = "1BHBGM", Abteilung = "HBGM", Jahrgang = 1},
                new Klasse{Name = "2AHBGM", Abteilung = "HBGM", Jahrgang = 2},
                new Klasse{Name = "2BHBGM", Abteilung = "HBGM", Jahrgang = 2}
            };
            Klassen.AddRange(klassen);
            var bewerbe = new List<Bewerb>
            {
                new Bewerb{BewerbId = 1001, Name = "100m Lauf"},
                new Bewerb{BewerbId = 1002,Name = "400m Lauf"},
                new Bewerb{BewerbId = 1003,Name = "5000m Lauf"}
            };
            Bewerbe.AddRange(bewerbe);
            SaveChanges();

            int schuelerId = 1001;
            var schuelerFaker = new Faker<Schueler>()
                .Rules((f, s) =>
                {
                    Name.Gender gender = f.Random.Enum<Name.Gender>();
                    s.SchuelerId = schuelerId++;
                    s.Vorname = f.Name.FirstName(gender);
                    s.Zuname = f.Name.LastName(gender);
                    s.Geschlecht = gender == Name.Gender.Female ? "w" : "m";
                    s.Klasse = f.Random.ListItem(klassen);
                });
            var schueler = schuelerFaker.Generate(100);
            Schueler.AddRange(schueler);
            SaveChanges();

            var ergebnisse = new List<Ergebnis>();
            foreach (int d in Enumerable.Range(1, 3))
            {
                foreach (Bewerb b in bewerbe)
                {
                    foreach (Schueler s in f.Random.ListItems(schueler, (int)(schueler.Count * 0.8)))
                    {
                        ergebnisse.Add(new Ergebnis
                        {
                            Schueler = s,
                            Bewerb = b,
                            Durchgang = d,
                            Zeit =
                                b.Name == "100m Lauf" ? Math.Round(f.Random.GaussianDecimal(15, 1.5), 2) :
                                b.Name == "400m Lauf" ? Math.Round(f.Random.GaussianDecimal(80, 8), 2) :
                                Math.Round(f.Random.GaussianDecimal(1100, 110), 2)
                        });
                    }
                }
            }
            Ergebnisse.AddRange(ergebnisse.Where(e => e.Durchgang <= 2));
            SaveChanges();

            if (writeFile)
            {
                // Die Daten UTF8 codiert schreiben. Der Zeilenumbruch wird je nach Betriebssystem
                // verwendet (\r\n oder \n).
                Console.WriteLine("Schreibe die TXT Datei in ergebnisse.txt");
                using (var fileStream = new StreamWriter("ergebnisse.txt", false, Encoding.UTF8))
                {
                    fileStream.WriteLine(string.Join("\t", "SCHUELER_ID", "SCHUELER_NAME", "BEWERB_ID", "BEWERB", "ZEIT", "DURCHGANG"));
                    foreach (var e in ergebnisse)
                        fileStream.WriteLine(string.Join("\t",
                            e.Schueler.SchuelerId,
                            e.Schueler.Zuname,
                            e.Bewerb.BewerbId,
                            e.Bewerb.Name,
                            e.Zeit,
                            e.Durchgang));
                }
            }
        }
    }
}
