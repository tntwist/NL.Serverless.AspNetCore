using System.Linq;
using NL.Serverless.AspNetCore.WebApp.ORM.Entities;

namespace NL.Serverless.AspNetCore.WebApp.ORM
{
    public static class DataSeeder
    {
        public static void Seed(WebAppDbContext dbContext) 
        {
            if (dbContext.ValueEntries.Any()) return;

            dbContext.ValueEntries.AddRange(
                new ValueEntry { Id = 1, Value = "This is value 1" },
                new ValueEntry { Id = 2, Value = "This is value 2" },
                new ValueEntry { Id = 3, Value = "This is value 3" },
                new ValueEntry { Id = 4, Value = "This is value 4" },
                new ValueEntry { Id = 5, Value = "This is value 5" }
            );

            dbContext.SaveChanges();
        }
    }
}
