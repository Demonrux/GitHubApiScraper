using InternetTechLab1.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetTechLab1.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<GitHubRepository> GitHubRepos { get; set; }
        public DbSet<ScrapedPage> ScrapedPages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InternetTechLab1.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO
        }
    }
}