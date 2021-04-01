using Microsoft.EntityFrameworkCore;
using NL.Serverless.AspNetCore.WebApp.ORM.Entities;

namespace NL.Serverless.AspNetCore.WebApp.ORM
{
    public class WebAppDbContext : DbContext
    {
        public virtual DbSet<ValueEntry> ValueEntries { get; set; }

        public WebAppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected WebAppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueEntry>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(128);
            });
        }
    }
}
