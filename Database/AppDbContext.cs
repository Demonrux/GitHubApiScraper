using InternetTechLab1.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetTechLab1.Database
{
    public class AppDbContext : DbContext
    {
        private readonly string _databaseName;

        public DbSet<GitHubRepository> GitHubRepos { get; set; }
        public DbSet<ScrapedPage> ScrapedPages { get; set; }

        public AppDbContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_databaseName}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO
        }
    }
}