using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace Tests
{
    [TestClass]
    public class InMemoryTests
    {
        //Use InMemory provider
        [TestMethod]
        public void CanInsertSamuraiIntoDatabase()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("CanInsertSamurai");
            using var context = new SamuraiContext();
            var samurai = new Samurai();
            context.Samurais.Add(samurai);
            Assert.AreEqual(EntityState.Added, context.Entry(samurai).State);
        }
    }
}