using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace ConsoleApp
{
    internal class Program
    {
        //private static SamuraiContext _context = new SamuraiContext();
        private static SamuraiContext _context = new SamuraiContext(new DbContextOptions<SamuraiContext>());

        private static void Main(string[] args)
        {
            _context.Database.EnsureCreated();
            GetSamurais("Before Add:");
            AddSamurai();
            InsertMultipleSamurais();
            InsertVariousTypes();

            GetSamurais("After Add:");
            Console.Write("Press any key...");
            Console.ReadKey();
            QueryFilters();
            RetrieveAndUpdateSamurai();
            RetrieveAndUpdateMultipleSamurais();
            RetrieveAndDeleteASamurai();
            QueryAndUpdateBattle_Disconnected();
            //Interacting with Related Data
            InsertNewSamuraiWithAQuote();
            AddQuoteToExistingSamuraiNotTracked(1);
            EagerLoadSamuraiWithQuotes();
            ProjectSomeProperties();
            ExplicitLoadQuotes();
            GetSamuraiWithBattles();
            //Working with Views and Stored Procedures and Raw SQL
            QuerySamuraiBattleStats();
            QueryUsingRawSql();
            QueryUsingRawSqlWithInterpolation();
            ExecuteSomeRawSql();
        }

        private static void ExecuteSomeRawSql()
        {
            var samuraiId = 22;
            var x = _context.Database.ExecuteSqlRaw("EXEC DeleteQuotesForSamurai {0}", samuraiId);
        }

        private static void QueryUsingRawSqlWithInterpolation()
        {
            var name = "Kikuchyo";
            var samurais = _context.Samurais
                .FromSqlInterpolated($"Select * from Samurais Where Name = {name}")
                .ToList();
        }

        private static void QueryUsingRawSql()
        {
            var samurais = _context.Samurais.FromSqlRaw("Select * From Samurais").ToList();
        }

        private static void QuerySamuraiBattleStats()
        {
            var stats = _context.SamuraiBattleStats.ToList();
        }

        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattles = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle)
                .FirstOrDefault(samurai => samurai.Id == 2);

            var samuraiWithBattlesCleaner = _context.Samurais.Where(s => s.Id == 2)
                .Select(s => new
                {
                    Samurai = s,
                    Battles = s.SamuraiBattles.Select(sb => sb.Battle)
                })
                .FirstOrDefault();
        }

        private static void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name.Contains("Julie"));
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.Horse).Load();
        }

        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais
                .Select(s => new {s.Id, s.Name, s.Quotes})
                .ToList();
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais
                .Where(s => s.Name.Contains("Julie "))
                .Include(s => s.Quotes).ToList();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you"
            });
            using (var newContextInstance = new SamuraiContext(new DbContextOptions<SamuraiContext>()))
            {
                newContextInstance.Samurais.Attach(samurai);
                newContextInstance.SaveChanges();
            }
        }

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote{ Text = "I`ve come to save you" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattle_Disconnected()
        {
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);
            using (var newContextInstance = new SamuraiContext(new DbContextOptions<SamuraiContext>()))
            {
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }

        private static void RetrieveAndDeleteASamurai()
        {
            var samurai = _context.Samurais.Find(18);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.Skip(1).Take(3).ToList();
            samurais.ForEach(s => s.Name = "San");
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }

        private static void QueryFilters()
        {
            var samurais = _context.Samurais
                .Where(s => s.Name == "Sampson")
                .ToList();
            var samurais1 = _context.Samurais.Where(s =>
                EF.Functions.Like(s.Name, "J%")).ToList();
        }

        private static void InsertMultipleSamurais()
        {
            var samurai1 = new Samurai { Name = "Sampson" };
            var samurai2 = new Samurai { Name = "Tasha" };
            _context.Samurais.AddRange(samurai1, samurai2);
            _context.SaveChanges();
        }

        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "Kikuchio" };
            var clan = new Clan{ ClanName = "Imperial Clan" };
            _context.AddRange(samurai, clan);
            _context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai {Name = "Sampson"};
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
    }
}